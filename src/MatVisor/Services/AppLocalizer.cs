namespace MatVisor;

internal enum AppLanguage
{
    Spanish,
    English
}

internal enum UiText
{
    OpenFile,
    Subtitle,
    File,
    NineColumns,
    EightColumns,
    SaveEightColumns,
    GenerateAnalysis,
    ZoomOut,
    ZoomIn,
    ResetZoom,
    InitialFilePrompt,
    InitialStatus,
    PreparingFile,
    FileNotFound,
    ReadingMatrix,
    OpenFileFailed,
    OriginalView,
    OriginalMatrixStatus,
    EightColumnView,
    EightColumnStatus,
    AnalysisCreated,
    AnalysisFailed,
    CannotOpenPicker,
    OpenPickerTitle,
    MatFile,
    CannotSavePicker,
    SavePickerTitle,
    SaveSuggestedSuffix,
    ChooseDifferentFile,
    SavingEightColumns,
    EightColumnsSaved,
    SaveFailed,
    MatrixTitle,
    CurrentView,
    ColumnPrefix,
    AnalysisWindowTitle,
    AnalysisTitle,
    AnalysisDescription,
    AnalysisSource,
    AnalysisRequiresEightOrNineColumns,
    ConversionRequiresNineColumns,
    NoValidMatrix
}

internal sealed class AppLocalizer
{
    private static readonly IReadOnlyDictionary<UiText, string> Spanish = new Dictionary<UiText, string>
    {
        [UiText.OpenFile] = "Abrir archivo .mat…",
        [UiText.Subtitle] = "Inspección rápida de matrices MATLAB, sin abrir MATLAB",
        [UiText.File] = "Archivo",
        [UiText.NineColumns] = "9 columnas",
        [UiText.EightColumns] = "8 columnas",
        [UiText.SaveEightColumns] = "Guardar matriz de 8 columnas…",
        [UiText.GenerateAnalysis] = "Generar análisis (cols. 9–12)",
        [UiText.ZoomOut] = "− Zoom",
        [UiText.ZoomIn] = "+ Zoom",
        [UiText.ResetZoom] = "Restablecer",
        [UiText.InitialFilePrompt] = "Elige un archivo .mat para comenzar.",
        [UiText.InitialStatus] = "También puedes iniciar MatVisor entregándole la ruta de un archivo .mat.",
        [UiText.PreparingFile] = "Preparando la lectura del archivo…",
        [UiText.FileNotFound] = "No existe el archivo indicado.",
        [UiText.ReadingMatrix] = "Leyendo la primera matriz numérica…",
        [UiText.OpenFileFailed] = "No se pudo abrir el archivo: {0}",
        [UiText.OriginalView] = "{0} columnas (original)",
        [UiText.OriginalMatrixStatus] = "Matriz original cargada. Arrastra sobre las celdas para seleccionar un rango; usa Ctrl+C o ⌘C para copiarlo.",
        [UiText.EightColumnView] = "8 columnas",
        [UiText.EightColumnStatus] = "Vista de 8 columnas: se quitaron {0} filas cuyo valor en la columna 9 es 2 y la columna 1 fue renumerada. La matriz original no fue modificada.",
        [UiText.AnalysisCreated] = "Análisis generado en una ventana independiente: {0} valores con C=0 y {1} con C=1.",
        [UiText.AnalysisFailed] = "No se pudo generar el análisis: {0}",
        [UiText.CannotOpenPicker] = "Este sistema no permite abrir el selector de archivos.",
        [UiText.OpenPickerTitle] = "Abrir archivo MATLAB",
        [UiText.MatFile] = "Archivo MATLAB",
        [UiText.CannotSavePicker] = "Este sistema no permite elegir dónde guardar el archivo.",
        [UiText.SavePickerTitle] = "Guardar matriz de 8 columnas",
        [UiText.SaveSuggestedSuffix] = "_8col.mat",
        [UiText.ChooseDifferentFile] = "Elige un nombre distinto para conservar el archivo original.",
        [UiText.SavingEightColumns] = "Guardando la matriz de 8 columnas…",
        [UiText.EightColumnsSaved] = "Matriz de 8 columnas guardada como {0}.",
        [UiText.SaveFailed] = "No se pudo guardar la matriz: {0}",
        [UiText.MatrixTitle] = "Nombre: {0} | Tamaño: ({1}, {2})",
        [UiText.CurrentView] = "Vista actual: {0}",
        [UiText.ColumnPrefix] = "Col",
        [UiText.AnalysisWindowTitle] = "Resultados del análisis — MatVisor",
        [UiText.AnalysisTitle] = "Resultados del análisis",
        [UiText.AnalysisDescription] = "Columnas 9 y 10: valores de cada conjunto y su promedio en negritas. Columnas 11 y 12: resumen de los tres promedios.",
        [UiText.AnalysisSource] = "Archivo: {0} · {1} valores con C=0 y {2} con C=1.",
        [UiText.AnalysisRequiresEightOrNineColumns] = "El análisis requiere una matriz de 8 o 9 columnas.",
        [UiText.ConversionRequiresNineColumns] = "La conversión a 8 columnas requiere una matriz de exactamente 9 columnas.",
        [UiText.NoValidMatrix] = "No se encontró una matriz numérica bidimensional válida en el archivo."
    };

    private static readonly IReadOnlyDictionary<UiText, string> English = new Dictionary<UiText, string>
    {
        [UiText.OpenFile] = "Open .mat file…",
        [UiText.Subtitle] = "Quick inspection of MATLAB matrices without opening MATLAB",
        [UiText.File] = "File",
        [UiText.NineColumns] = "9 columns",
        [UiText.EightColumns] = "8 columns",
        [UiText.SaveEightColumns] = "Save 8-column matrix…",
        [UiText.GenerateAnalysis] = "Generate analysis (cols. 9–12)",
        [UiText.ZoomOut] = "− Zoom",
        [UiText.ZoomIn] = "+ Zoom",
        [UiText.ResetZoom] = "Reset",
        [UiText.InitialFilePrompt] = "Choose a .mat file to begin.",
        [UiText.InitialStatus] = "You can also start MatVisor by passing it the path to a .mat file.",
        [UiText.PreparingFile] = "Preparing to read the file…",
        [UiText.FileNotFound] = "The specified file does not exist.",
        [UiText.ReadingMatrix] = "Reading the first numeric matrix…",
        [UiText.OpenFileFailed] = "Could not open the file: {0}",
        [UiText.OriginalView] = "{0} columns (original)",
        [UiText.OriginalMatrixStatus] = "Original matrix loaded. Drag over cells to select a range; use Ctrl+C or ⌘C to copy it.",
        [UiText.EightColumnView] = "8 columns",
        [UiText.EightColumnStatus] = "8-column view: removed {0} rows whose value in column 9 is 2 and renumbered column 1. The original matrix was not modified.",
        [UiText.AnalysisCreated] = "Analysis generated in a separate window: {0} values with C=0 and {1} with C=1.",
        [UiText.AnalysisFailed] = "Could not generate the analysis: {0}",
        [UiText.CannotOpenPicker] = "This system cannot open the file picker.",
        [UiText.OpenPickerTitle] = "Open MATLAB file",
        [UiText.MatFile] = "MATLAB file",
        [UiText.CannotSavePicker] = "This system cannot choose where to save the file.",
        [UiText.SavePickerTitle] = "Save 8-column matrix",
        [UiText.SaveSuggestedSuffix] = "_8col.mat",
        [UiText.ChooseDifferentFile] = "Choose a different name to preserve the original file.",
        [UiText.SavingEightColumns] = "Saving 8-column matrix…",
        [UiText.EightColumnsSaved] = "8-column matrix saved as {0}.",
        [UiText.SaveFailed] = "Could not save the matrix: {0}",
        [UiText.MatrixTitle] = "Name: {0} | Size: ({1}, {2})",
        [UiText.CurrentView] = "Current view: {0}",
        [UiText.ColumnPrefix] = "Col",
        [UiText.AnalysisWindowTitle] = "Analysis results — MatVisor",
        [UiText.AnalysisTitle] = "Analysis results",
        [UiText.AnalysisDescription] = "Columns 9 and 10: values in each group and their bold average. Columns 11 and 12: summary of the three averages.",
        [UiText.AnalysisSource] = "File: {0} · {1} values with C=0 and {2} with C=1.",
        [UiText.AnalysisRequiresEightOrNineColumns] = "The analysis requires an 8- or 9-column matrix.",
        [UiText.ConversionRequiresNineColumns] = "The 8-column conversion requires a matrix with exactly 9 columns.",
        [UiText.NoValidMatrix] = "No valid two-dimensional numeric matrix was found in the file."
    };

    public AppLanguage CurrentLanguage { get; private set; } = AppLanguage.English;

    public event EventHandler? LanguageChanged;

    public string Get(UiText key) => (CurrentLanguage == AppLanguage.Spanish ? Spanish : English)[key];

    public string Format(UiText key, params object[] values) => string.Format(Get(key), values);

    public void SetLanguage(AppLanguage language)
    {
        if (CurrentLanguage == language)
        {
            return;
        }

        CurrentLanguage = language;
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}
