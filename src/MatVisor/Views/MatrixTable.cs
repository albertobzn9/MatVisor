using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using MathNet.Numerics.LinearAlgebra;
using System.Globalization;
using System.Text;

namespace MatVisor;

internal sealed class MatrixTable : Grid
{
    private const double BaseHeaderHeight = 32;
    private const double BaseRowHeight = 28;
    private readonly List<double> _columnWidths = [];
    private Border[,]? _cells;
    private Matrix<double>? _matrix;
    private int _firstColumnNumber = 1;
    private IReadOnlyDictionary<(int Row, int Column), string> _displayValues =
        new Dictionary<(int Row, int Column), string>();
    private IReadOnlySet<(int Row, int Column)> _boldCells =
        new HashSet<(int Row, int Column)>();
    private double _zoomFactor = 1;
    private (int Row, int Column)? _anchor;
    private (int Row, int Column)? _selectionEnd;

    public MatrixTable()
    {
        Background = Brushes.Transparent;
        Focusable = true;
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
        KeyDown += OnKeyDown;
    }

    public void Display(
        Matrix<double> matrix,
        int firstColumnNumber = 1,
        IReadOnlyDictionary<(int Row, int Column), string>? displayValues = null,
        IReadOnlySet<(int Row, int Column)>? boldCells = null)
    {
        _matrix = matrix;
        _firstColumnNumber = firstColumnNumber;
        _displayValues = displayValues ?? new Dictionary<(int Row, int Column), string>();
        _boldCells = boldCells ?? new HashSet<(int Row, int Column)>();
        _anchor = null;
        _selectionEnd = null;
        _columnWidths.Clear();
        _cells = new Border[matrix.RowCount, matrix.ColumnCount];
        Children.Clear();
        ColumnDefinitions.Clear();
        RowDefinitions.Clear();
        RowDefinitions.Add(new RowDefinition(HeaderHeight, GridUnitType.Pixel));

        for (var column = 0; column < matrix.ColumnCount; column++)
        {
            var width = CalculateColumnWidth(matrix, column);
            _columnWidths.Add(width);
            ColumnDefinitions.Add(new ColumnDefinition(width, GridUnitType.Pixel));
            AddHeader(column);
        }

        for (var row = 0; row < matrix.RowCount; row++)
        {
            RowDefinitions.Add(new RowDefinition(RowHeight, GridUnitType.Pixel));

            for (var column = 0; column < matrix.ColumnCount; column++)
            {
                AddCell(matrix, row, column);
            }
        }
    }

    public int ZoomPercent => (int)Math.Round(_zoomFactor * 100);

    public void ZoomOut() => ChangeZoom(-0.1);

    public void ZoomIn() => ChangeZoom(0.1);

    public void ResetZoom()
    {
        if (_zoomFactor != 1)
        {
            _zoomFactor = 1;
            RefreshDisplay();
        }
    }

    private double HeaderHeight => BaseHeaderHeight * _zoomFactor;

    private double RowHeight => BaseRowHeight * _zoomFactor;

    private double CalculateColumnWidth(Matrix<double> matrix, int column)
    {
        var longestText = $"Col {column + _firstColumnNumber}".Length;

        for (var row = 0; row < matrix.RowCount; row++)
        {
            longestText = Math.Max(longestText, GetDisplayValue(row, column).Length);
        }

        return Math.Clamp(((longestText * 9) + 28) * _zoomFactor, 56, 240);
    }

    private void AddHeader(int column)
    {
        var header = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#E8E8E8")),
            BorderBrush = new SolidColorBrush(Color.Parse("#D1D1D1")),
            BorderThickness = new Thickness(0, 0, 1, 1),
            Child = new TextBlock
            {
                FontWeight = FontWeight.SemiBold,
                FontSize = 14 * _zoomFactor,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Text = $"Col {column + _firstColumnNumber}"
            }
        };

        SetRow(header, 0);
        SetColumn(header, column);
        Children.Add(header);
    }

    private void AddCell(Matrix<double> matrix, int row, int column)
    {
        var cell = new Border
        {
            Background = row % 2 == 0 ? Brushes.White : new SolidColorBrush(Color.Parse("#F8F8F8")),
            BorderBrush = new SolidColorBrush(Color.Parse("#EBEBEB")),
            BorderThickness = new Thickness(0, 0, 1, 1),
            Child = new TextBlock
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Text = GetDisplayValue(row, column),
                TextTrimming = TextTrimming.CharacterEllipsis,
                FontWeight = _boldCells.Contains((row, column)) ? FontWeight.Bold : FontWeight.Normal,
                FontSize = 14 * _zoomFactor
            }
        };

        SetRow(cell, row + 1);
        SetColumn(cell, column);
        Children.Add(cell);
        _cells![row, column] = cell;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs eventArgs)
    {
        if (!eventArgs.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        Focus();
        var cell = GetCellAt(eventArgs.GetPosition(this));
        if (cell is null)
        {
            return;
        }

        _anchor = cell;
        _selectionEnd = cell;
        eventArgs.Pointer.Capture(this);
        UpdateSelectionVisuals();
        eventArgs.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs eventArgs)
    {
        if (_anchor is null || !eventArgs.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        var cell = GetCellAt(eventArgs.GetPosition(this));
        if (cell is not null && cell != _selectionEnd)
        {
            _selectionEnd = cell;
            UpdateSelectionVisuals();
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs eventArgs)
    {
        if (eventArgs.Pointer.Captured == this)
        {
            eventArgs.Pointer.Capture(null);
        }
    }

    private async void OnKeyDown(object? sender, KeyEventArgs eventArgs)
    {
        var commandModifier = eventArgs.KeyModifiers.HasFlag(KeyModifiers.Control)
                              || eventArgs.KeyModifiers.HasFlag(KeyModifiers.Meta);
        if (!commandModifier || _matrix is null)
        {
            return;
        }

        if (eventArgs.Key == Key.A)
        {
            _anchor = (0, 0);
            _selectionEnd = (_matrix.RowCount - 1, _matrix.ColumnCount - 1);
            UpdateSelectionVisuals();
            eventArgs.Handled = true;
            return;
        }

        if (eventArgs.Key == Key.C && _anchor is not null && _selectionEnd is not null)
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard is not null)
            {
                var data = new DataTransfer();
                data.Add(DataTransferItem.CreateText(BuildClipboardText()));
                await clipboard.SetDataAsync(data);
            }

            eventArgs.Handled = true;
        }
    }

    private (int Row, int Column)? GetCellAt(Point position)
    {
        if (_matrix is null || position.Y < HeaderHeight)
        {
            return null;
        }

        var row = (int)((position.Y - HeaderHeight) / RowHeight);
        if (row < 0 || row >= _matrix.RowCount)
        {
            return null;
        }

        var left = 0d;
        for (var column = 0; column < _columnWidths.Count; column++)
        {
            if (position.X >= left && position.X < left + _columnWidths[column])
            {
                return (row, column);
            }

            left += _columnWidths[column];
        }

        return null;
    }

    private void UpdateSelectionVisuals()
    {
        if (_cells is null || _anchor is null || _selectionEnd is null)
        {
            return;
        }

        var startRow = Math.Min(_anchor.Value.Row, _selectionEnd.Value.Row);
        var endRow = Math.Max(_anchor.Value.Row, _selectionEnd.Value.Row);
        var startColumn = Math.Min(_anchor.Value.Column, _selectionEnd.Value.Column);
        var endColumn = Math.Max(_anchor.Value.Column, _selectionEnd.Value.Column);

        for (var row = 0; row < _cells.GetLength(0); row++)
        {
            for (var column = 0; column < _cells.GetLength(1); column++)
            {
                var selected = row >= startRow && row <= endRow && column >= startColumn && column <= endColumn;
                _cells[row, column].Background = selected
                    ? new SolidColorBrush(Color.Parse("#B9D9FF"))
                    : row % 2 == 0 ? Brushes.White : new SolidColorBrush(Color.Parse("#F8F8F8"));
            }
        }
    }

    private string BuildClipboardText()
    {
        var startRow = Math.Min(_anchor!.Value.Row, _selectionEnd!.Value.Row);
        var endRow = Math.Max(_anchor!.Value.Row, _selectionEnd!.Value.Row);
        var startColumn = Math.Min(_anchor!.Value.Column, _selectionEnd!.Value.Column);
        var endColumn = Math.Max(_anchor!.Value.Column, _selectionEnd!.Value.Column);
        var result = new StringBuilder();

        for (var row = startRow; row <= endRow; row++)
        {
            if (row > startRow)
            {
                result.AppendLine();
            }

            for (var column = startColumn; column <= endColumn; column++)
            {
                if (column > startColumn)
                {
                    result.Append('\t');
                }

                result.Append(GetDisplayValue(row, column));
            }
        }

        return result.ToString();
    }

    private string GetDisplayValue(int row, int column) =>
        _displayValues.TryGetValue((row, column), out var displayValue)
            ? displayValue
            : FormatValue(_matrix![row, column]);

    private void ChangeZoom(double change)
    {
        var newZoom = Math.Clamp(Math.Round(_zoomFactor + change, 1), 0.6, 1.4);
        if (newZoom != _zoomFactor)
        {
            _zoomFactor = newZoom;
            RefreshDisplay();
        }
    }

    private void RefreshDisplay()
    {
        if (_matrix is not null)
        {
            Display(_matrix, _firstColumnNumber, _displayValues, _boldCells);
        }
    }

    private static string FormatValue(double value) => double.IsNaN(value)
        ? string.Empty
        : value.ToString("0.00", CultureInfo.CurrentCulture);
}
