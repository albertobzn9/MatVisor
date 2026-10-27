using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using MathNet.Numerics.Data.Matlab;
using MathNet.Numerics.LinearAlgebra;
using System.IO;

namespace MatVisor;

public partial class MainWindow : Window
{
    private readonly string? _filePath;
    private readonly AppLocalizer _localizer;
    private Matrix<double>? _originalMatrix;
    private Matrix<double>? _eightColumnMatrix;
    private Matrix<double>? _displayedMatrix;
    private AnalysisWindow? _analysisWindow;
    private string? _loadedFilePath;
    private int _removedRowCount;
    private MatrixView _matrixView = MatrixView.Original;
    private bool _applyingLanguage;

    public MainWindow()
        : this([])
    {
    }

    public MainWindow(string[] launchArguments)
    {
        InitializeComponent();
        _localizer = ((App)Application.Current!).Localizer;
        _filePath = launchArguments.FirstOrDefault(argument => !argument.StartsWith("-", StringComparison.Ordinal));
        _localizer.LanguageChanged += Localizer_LanguageChanged;
        Opened += async (_, _) => await LoadLaunchFileAsync();
        Closed += (_, _) => _localizer.LanguageChanged -= Localizer_LanguageChanged;
        ApplyLocalizedText();
        ShowInitialStatus();
    }

    private void Localizer_LanguageChanged(object? sender, EventArgs eventArgs) => ApplyLocalizedText();

    private void ApplyLocalizedText()
    {
        OpenFileButton.Content = _localizer.Get(UiText.OpenFile);
        SubtitleText.Text = _localizer.Get(UiText.Subtitle);
        FileLabelText.Text = _localizer.Get(UiText.File);
        ShowNineColumnsButton.Content = _localizer.Get(UiText.NineColumns);
        ShowEightColumnsButton.Content = _localizer.Get(UiText.EightColumns);
        SaveEightColumnsButton.Content = _localizer.Get(UiText.SaveEightColumns);
        GenerateAnalysisButton.Content = _localizer.Get(UiText.GenerateAnalysis);
        ZoomOutButton.Content = _localizer.Get(UiText.ZoomOut);
        ZoomInButton.Content = _localizer.Get(UiText.ZoomIn);
        ResetZoomButton.Content = _localizer.Get(UiText.ResetZoom);

        _applyingLanguage = true;
        LanguageToggleButton.IsChecked = _localizer.CurrentLanguage == AppLanguage.English;
        _applyingLanguage = false;
        UpdateLanguageToggleAppearance();
        MatrixTable.SetColumnHeaderPrefix(_localizer.Get(UiText.ColumnPrefix));

        if (_displayedMatrix is null)
        {
            ShowInitialStatus();
            return;
        }

        UpdateWindowTitle();
        UpdateDisplayedMatrixText();
    }

    private void LanguageToggleButton_IsCheckedChanged(object? sender, RoutedEventArgs eventArgs)
    {
        if (!_applyingLanguage)
        {
            _localizer.SetLanguage(LanguageToggleButton.IsChecked == true
                ? AppLanguage.English
                : AppLanguage.Spanish);
        }
    }

    private void UpdateLanguageToggleAppearance()
    {
        var englishSelected = _localizer.CurrentLanguage == AppLanguage.English;
        SpanishLanguageText.FontWeight = englishSelected ? FontWeight.Normal : FontWeight.Bold;
        EnglishLanguageText.FontWeight = englishSelected ? FontWeight.Bold : FontWeight.Normal;
        SpanishLanguageText.Foreground = new SolidColorBrush(Color.Parse(englishSelected ? "#6B7280" : "#0F6CBD"));
        EnglishLanguageText.Foreground = new SolidColorBrush(Color.Parse(englishSelected ? "#0F6CBD" : "#6B7280"));
    }

    private void ShowInitialStatus()
    {
        if (string.IsNullOrWhiteSpace(_filePath))
        {
            FilePathText.Text = _localizer.Get(UiText.InitialFilePrompt);
            StatusText.Text = _localizer.Get(UiText.InitialStatus);
            Title = "MatVisor";
            return;
        }

        FilePathText.Text = Path.GetFullPath(_filePath);
        StatusText.Text = _localizer.Get(UiText.PreparingFile);
        Title = $"MatVisor — {Path.GetFileName(_filePath)}";
    }

    private async Task LoadLaunchFileAsync()
    {
        if (!string.IsNullOrWhiteSpace(_filePath))
        {
            await OpenFileAsync(Path.GetFullPath(_filePath));
        }
    }

    private async void OpenFileButton_Click(object? sender, RoutedEventArgs eventArgs)
    {
        if (!StorageProvider.CanOpen)
        {
            StatusText.Text = _localizer.Get(UiText.CannotOpenPicker);
            return;
        }

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = _localizer.Get(UiText.OpenPickerTitle),
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType(_localizer.Get(UiText.MatFile))
                {
                    Patterns = ["*.mat"]
                }
            ]
        });

        var file = files.FirstOrDefault();
        if (file is not null)
        {
            await OpenFileAsync(file.Path.LocalPath);
        }
    }

    public async Task OpenFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.GetFullPath(filePath);
            FilePathText.Text = fullPath;

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(_localizer.Get(UiText.FileNotFound), fullPath);
            }

            StatusText.Text = _localizer.Get(UiText.ReadingMatrix);
            var matrix = await Task.Run(() => MatFileLoader.LoadFirstNumericMatrix(fullPath));
            ShowMatrix(matrix, fullPath);
        }
        catch (Exception exception)
        {
            MatrixPanel.IsVisible = false;
            StatusText.Text = _localizer.Format(UiText.OpenFileFailed, exception.Message);
        }
    }

    private void ShowMatrix(Matrix<double> matrix, string filePath)
    {
        _originalMatrix = matrix;
        _eightColumnMatrix = null;
        _displayedMatrix = null;
        _loadedFilePath = filePath;
        _removedRowCount = 0;
        _matrixView = MatrixView.Original;
        ColumnModePanel.IsVisible = matrix.ColumnCount == 9;
        GenerateAnalysisButton.IsVisible = matrix.ColumnCount is 8 or 9;
        ZoomPanel.IsVisible = true;
        SaveEightColumnsButton.IsVisible = false;
        CloseAnalysisWindow();
        ShowNineColumns();
        UpdateWindowTitle();
        MatrixPanel.IsVisible = true;
    }

    private void ShowNineColumns_Click(object? sender, RoutedEventArgs eventArgs) => ShowNineColumns();

    private void ShowEightColumns_Click(object? sender, RoutedEventArgs eventArgs)
    {
        if (_originalMatrix is null || _originalMatrix.ColumnCount != 9)
        {
            return;
        }

        _eightColumnMatrix = MatrixTransformer.CreateEightColumnView(_originalMatrix, out _removedRowCount);
        _matrixView = MatrixView.EightColumns;
        SaveEightColumnsButton.IsVisible = true;
        DisplayMatrix(_eightColumnMatrix);
    }

    private void ShowNineColumns()
    {
        if (_originalMatrix is null)
        {
            return;
        }

        _matrixView = MatrixView.Original;
        SaveEightColumnsButton.IsVisible = false;
        DisplayMatrix(_originalMatrix);
    }

    private void GenerateAnalysis_Click(object? sender, RoutedEventArgs eventArgs)
    {
        if (_displayedMatrix is null)
        {
            return;
        }

        try
        {
            var analysis = MatrixAnalysisBuilder.Create(_displayedMatrix);
            CloseAnalysisWindow();
            var analysisWindow = new AnalysisWindow(analysis, _loadedFilePath ?? string.Empty);
            _analysisWindow = analysisWindow;
            analysisWindow.Closed += (_, _) =>
            {
                if (ReferenceEquals(_analysisWindow, analysisWindow))
                {
                    _analysisWindow = null;
                }
            };
            analysisWindow.Show(this);
            StatusText.Text = _localizer.Format(UiText.AnalysisCreated, analysis.CeroCount, analysis.UnoCount);
        }
        catch (Exception exception)
        {
            StatusText.Text = _localizer.Format(UiText.AnalysisFailed, exception.Message);
        }
    }

    private void ZoomOut_Click(object? sender, RoutedEventArgs eventArgs)
    {
        MatrixTable.ZoomOut();
        UpdateZoomText();
    }

    private void ZoomIn_Click(object? sender, RoutedEventArgs eventArgs)
    {
        MatrixTable.ZoomIn();
        UpdateZoomText();
    }

    private void ResetZoom_Click(object? sender, RoutedEventArgs eventArgs)
    {
        MatrixTable.ResetZoom();
        UpdateZoomText();
    }

    private async void SaveEightColumns_Click(object? sender, RoutedEventArgs eventArgs)
    {
        if (_eightColumnMatrix is null || string.IsNullOrWhiteSpace(_loadedFilePath))
        {
            return;
        }

        if (!StorageProvider.CanSave)
        {
            StatusText.Text = _localizer.Get(UiText.CannotSavePicker);
            return;
        }

        var destination = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = _localizer.Get(UiText.SavePickerTitle),
            SuggestedFileName = $"{Path.GetFileNameWithoutExtension(_loadedFilePath)}{_localizer.Get(UiText.SaveSuggestedSuffix)}",
            DefaultExtension = "mat",
            FileTypeChoices =
            [
                new FilePickerFileType(_localizer.Get(UiText.MatFile))
                {
                    Patterns = ["*.mat"]
                }
            ],
            ShowOverwritePrompt = true
        });

        if (destination is null)
        {
            return;
        }

        if (string.Equals(destination.Path.LocalPath, _loadedFilePath, StringComparison.OrdinalIgnoreCase))
        {
            StatusText.Text = _localizer.Get(UiText.ChooseDifferentFile);
            return;
        }

        try
        {
            StatusText.Text = _localizer.Get(UiText.SavingEightColumns);
            await using var stream = await destination.OpenWriteAsync();
            await Task.Run(() => MatlabWriter.Write(stream, _eightColumnMatrix, "Resultados"));
            StatusText.Text = _localizer.Format(UiText.EightColumnsSaved, destination.Name);
        }
        catch (Exception exception)
        {
            StatusText.Text = _localizer.Format(UiText.SaveFailed, exception.Message);
        }
    }

    private void DisplayMatrix(Matrix<double> matrix)
    {
        _displayedMatrix = matrix;
        CloseAnalysisWindow();
        MatrixTable.Display(matrix);
        UpdateZoomText();
        UpdateDisplayedMatrixText();
    }

    private void UpdateDisplayedMatrixText()
    {
        if (_displayedMatrix is null)
        {
            return;
        }

        MatrixTitleText.Text = _localizer.Format(
            UiText.MatrixTitle,
            Path.GetFileNameWithoutExtension(_loadedFilePath) ?? string.Empty,
            _displayedMatrix.RowCount,
            _displayedMatrix.ColumnCount);
        ColumnModeText.Text = _localizer.Format(UiText.CurrentView, GetViewName());
        StatusText.Text = GetMatrixStatus();
    }

    private string GetViewName() => _matrixView == MatrixView.Original
        ? _localizer.Format(UiText.OriginalView, _originalMatrix?.ColumnCount ?? 0)
        : _localizer.Get(UiText.EightColumnView);

    private string GetMatrixStatus() => _matrixView == MatrixView.Original
        ? _localizer.Get(UiText.OriginalMatrixStatus)
        : _localizer.Format(UiText.EightColumnStatus, _removedRowCount);

    private void UpdateWindowTitle()
    {
        Title = string.IsNullOrWhiteSpace(_loadedFilePath)
            ? "MatVisor"
            : $"MatVisor — {Path.GetFileName(_loadedFilePath)}";
    }

    private void UpdateZoomText() => ZoomText.Text = $"{MatrixTable.ZoomPercent}%";

    private void CloseAnalysisWindow()
    {
        _analysisWindow?.Close();
        _analysisWindow = null;
    }

    private enum MatrixView
    {
        Original,
        EightColumns
    }
}
