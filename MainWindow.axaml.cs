using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MathNet.Numerics.Data.Matlab;
using MathNet.Numerics.LinearAlgebra;
using System.IO;

namespace VisorMat;

public partial class MainWindow : Window
{
    private readonly string? _filePath;
    private Matrix<double>? _originalMatrix;
    private Matrix<double>? _eightColumnMatrix;
    private string? _loadedFilePath;

    public MainWindow()
        : this([])
    {
    }

    public MainWindow(string[] launchArguments)
    {
        InitializeComponent();
        _filePath = launchArguments.FirstOrDefault(argument => !argument.StartsWith("-", StringComparison.Ordinal));
        Opened += async (_, _) => await LoadLaunchFileAsync();
        ShowInitialStatus();
    }

    private void ShowInitialStatus()
    {
        if (string.IsNullOrWhiteSpace(_filePath))
        {
            FilePathText.Text = "Elige un archivo .mat para comenzar.";
            StatusText.Text = "También puedes iniciar VisorMat entregándole la ruta de un archivo .mat.";
            return;
        }

        FilePathText.Text = Path.GetFullPath(_filePath);
        StatusText.Text = "Preparando la lectura del archivo…";
        Title = $"VisorMat — {Path.GetFileName(_filePath)}";
    }

    private async Task LoadLaunchFileAsync()
    {
        if (!string.IsNullOrWhiteSpace(_filePath))
        {
            await LoadFileAsync(Path.GetFullPath(_filePath));
        }
    }

    private async void OpenFileButton_Click(object? sender, RoutedEventArgs eventArgs)
    {
        if (!StorageProvider.CanOpen)
        {
            StatusText.Text = "Este sistema no permite abrir el selector de archivos.";
            return;
        }

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Abrir archivo MATLAB",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Archivo MATLAB")
                {
                    Patterns = ["*.mat"]
                }
            ]
        });

        var file = files.FirstOrDefault();
        if (file is not null)
        {
            await LoadFileAsync(file.Path.LocalPath);
        }
    }

    private async Task LoadFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.GetFullPath(filePath);
            FilePathText.Text = fullPath;

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("No existe el archivo indicado.", fullPath);
            }

            StatusText.Text = "Leyendo la primera matriz numérica…";
            var matrix = await Task.Run(() => MatFileLoader.LoadFirstNumericMatrix(fullPath));
            ShowMatrix(matrix, fullPath);
        }
        catch (Exception exception)
        {
            MatrixPanel.IsVisible = false;
            StatusText.Text = $"No se pudo abrir el archivo: {exception.Message}";
        }
    }

    private void ShowMatrix(Matrix<double> matrix, string filePath)
    {
        _originalMatrix = matrix;
        _eightColumnMatrix = null;
        _loadedFilePath = filePath;
        ColumnModePanel.IsVisible = matrix.ColumnCount == 9;
        SaveEightColumnsButton.IsVisible = false;
        ShowNineColumns();
        Title = $"VisorMat — {Path.GetFileName(filePath)}";
        MatrixPanel.IsVisible = true;
    }

    private void ShowNineColumns_Click(object? sender, RoutedEventArgs eventArgs) => ShowNineColumns();

    private void ShowEightColumns_Click(object? sender, RoutedEventArgs eventArgs)
    {
        if (_originalMatrix is null || _originalMatrix.ColumnCount != 9)
        {
            return;
        }

        var eightColumnMatrix = MatrixTransformer.CreateEightColumnView(_originalMatrix, out var removedRowCount);
        _eightColumnMatrix = eightColumnMatrix;
        SaveEightColumnsButton.IsVisible = true;

        DisplayMatrix(
            eightColumnMatrix,
            "8 columnas",
            $"Vista de 8 columnas: se quitaron {removedRowCount} filas cuyo valor en la columna 9 es 2 y la columna 1 fue renumerada. La matriz original no fue modificada.");
    }

    private void ShowNineColumns()
    {
        if (_originalMatrix is null)
        {
            return;
        }

        SaveEightColumnsButton.IsVisible = false;
        DisplayMatrix(
            _originalMatrix,
            $"{_originalMatrix.ColumnCount} columnas (original)",
            "Matriz original cargada. Arrastra sobre las celdas para seleccionar un rango; usa Ctrl+C o ⌘C para copiarlo.");
    }

    private async void SaveEightColumns_Click(object? sender, RoutedEventArgs eventArgs)
    {
        if (_eightColumnMatrix is null || string.IsNullOrWhiteSpace(_loadedFilePath))
        {
            return;
        }

        if (!StorageProvider.CanSave)
        {
            StatusText.Text = "Este sistema no permite elegir dónde guardar el archivo.";
            return;
        }

        var destination = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Guardar matriz de 8 columnas",
            SuggestedFileName = $"{Path.GetFileNameWithoutExtension(_loadedFilePath)}_8col.mat",
            DefaultExtension = "mat",
            FileTypeChoices =
            [
                new FilePickerFileType("Archivo MATLAB")
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
            StatusText.Text = "Elige un nombre distinto para conservar el archivo original.";
            return;
        }

        try
        {
            StatusText.Text = "Guardando la matriz de 8 columnas…";
            await using var stream = await destination.OpenWriteAsync();
            await Task.Run(() => MatlabWriter.Write(stream, _eightColumnMatrix, "Resultados"));
            StatusText.Text = $"Matriz de 8 columnas guardada como {destination.Name}.";
        }
        catch (Exception exception)
        {
            StatusText.Text = $"No se pudo guardar la matriz: {exception.Message}";
        }
    }

    private void DisplayMatrix(Matrix<double> matrix, string viewName, string status)
    {
        MatrixTable.Display(matrix);
        MatrixTitleText.Text = $"Nombre: {Path.GetFileNameWithoutExtension(_loadedFilePath)} | Tamaño: ({matrix.RowCount}, {matrix.ColumnCount})";
        ColumnModeText.Text = $"Vista actual: {viewName}";
        StatusText.Text = status;
    }
}
