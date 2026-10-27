using Avalonia;
using Avalonia.Controls;
using System.IO;

namespace MatVisor;

internal partial class AnalysisWindow : Window
{
    private readonly MatrixAnalysis _analysis;
    private readonly string _filePath;
    private readonly AppLocalizer _localizer;

    internal AnalysisWindow(MatrixAnalysis analysis, string filePath)
    {
        InitializeComponent();
        _analysis = analysis;
        _filePath = filePath;
        _localizer = ((App)Application.Current!).Localizer;
        _localizer.LanguageChanged += Localizer_LanguageChanged;
        Closed += (_, _) => _localizer.LanguageChanged -= Localizer_LanguageChanged;

        AnalysisTable.Display(
            _analysis.DisplayMatrix,
            firstColumnNumber: 9,
            displayValues: _analysis.DisplayValues,
            boldCells: _analysis.BoldCells);
        ApplyLocalizedText();
    }

    private void Localizer_LanguageChanged(object? sender, EventArgs eventArgs) => ApplyLocalizedText();

    private void ApplyLocalizedText()
    {
        Title = _localizer.Get(UiText.AnalysisWindowTitle);
        AnalysisTitleText.Text = _localizer.Get(UiText.AnalysisTitle);
        AnalysisDescriptionText.Text = _localizer.Get(UiText.AnalysisDescription);
        SourceText.Text = _localizer.Format(
            UiText.AnalysisSource,
            Path.GetFileName(_filePath),
            _analysis.CeroCount,
            _analysis.UnoCount);
        ZoomOutButton.Content = _localizer.Get(UiText.ZoomOut);
        ZoomInButton.Content = _localizer.Get(UiText.ZoomIn);
        ResetZoomButton.Content = _localizer.Get(UiText.ResetZoom);
        AnalysisTable.SetColumnHeaderPrefix(_localizer.Get(UiText.ColumnPrefix));
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
