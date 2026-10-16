using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MathNet.Numerics.LinearAlgebra;
using System.IO;

namespace VisorMat;

public partial class MainWindow : Window
{
    private readonly string? _filePath;

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
        MatrixTable.Display(matrix);
        MatrixTitleText.Text = $"Nombre: {Path.GetFileNameWithoutExtension(filePath)} | Tamaño: ({matrix.RowCount}, {matrix.ColumnCount})";
        StatusText.Text = "Matriz cargada. Arrastra sobre las celdas para seleccionar un rango; usa Ctrl+C o ⌘C para copiarlo.";
        Title = $"VisorMat — {Path.GetFileName(filePath)}";
        MatrixPanel.IsVisible = true;
    }
}
