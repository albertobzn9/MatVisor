using Avalonia.Controls;
using System.IO;

namespace MatVisor;

internal partial class AnalysisWindow : Window
{
    internal AnalysisWindow(MatrixAnalysis analysis, string filePath)
    {
        InitializeComponent();
        SourceText.Text = $"Archivo: {Path.GetFileName(filePath)} · " +
                          $"{analysis.CeroCount} valores con C=0 y {analysis.UnoCount} con C=1.";
        AnalysisTable.Display(
            analysis.DisplayMatrix,
            firstColumnNumber: 9,
            displayValues: analysis.DisplayValues,
            boldCells: analysis.BoldCells);
    }

    private void ZoomOut_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs eventArgs)
    {
        AnalysisTable.ZoomOut();
        UpdateZoomText();
    }

    private void ZoomIn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs eventArgs)
    {
        AnalysisTable.ZoomIn();
        UpdateZoomText();
    }

    private void ResetZoom_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs eventArgs)
    {
        AnalysisTable.ResetZoom();
        UpdateZoomText();
    }

    private void UpdateZoomText() => ZoomText.Text = $"{AnalysisTable.ZoomPercent}%";
}
