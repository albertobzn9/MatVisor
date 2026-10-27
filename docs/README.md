# MatVisor architecture

This folder records the current design so a new contributor can navigate and extend the application without reconstructing its behavior from the user interface.

## Application flow

1. `Program.cs` starts the Avalonia desktop lifetime.
2. `App.axaml.cs` creates `MainWindow` and receives `.mat` file activations from the operating system.
3. `MainWindow` asks `MatFileLoader` for the first compatible numeric matrix in the selected file.
4. `MatrixTable` renders the matrix, supports range selection, clipboard copying, and zoom.
5. The user can inspect the original matrix, create an 8-column view, save it, or open the derived analysis in `AnalysisWindow`.

## Source map

| Location | Responsibility |
| --- | --- |
| `src/MatVisor/Views/` | Avalonia windows and the reusable interactive matrix table. |
| `src/MatVisor/Models/MatrixAnalysis.cs` | Data returned by the grouped-analysis calculation. |
| `src/MatVisor/Services/MatFileLoader.cs` | Reads the first supported two-dimensional numeric matrix in a `.mat` file. |
| `src/MatVisor/Services/MatrixTransformer.cs` | Produces the derived 8-column view. |
| `src/MatVisor/Services/AppLocalizer.cs` | Central Spanish/English interface text and the shared language state. |
| `src/MatVisor/Assets/` | Application and package icons. |
| `packaging/macos/Info.plist` | macOS bundle metadata and `.mat` document association. |
| `scripts/package-release.sh` | Builds self-contained macOS and Windows distributables from macOS. |

## Matrix rules implemented today

### 9-column to 8-column view

This view is available only when the loaded matrix has exactly nine columns.

1. Remove each row whose ninth-column value is `2`.
2. Keep the first eight columns of every remaining row.
3. Replace column 1 with consecutive row numbers beginning at `1`.
4. Preserve the original matrix in memory; only the derived view is saved when the user chooses **Guardar matriz de 8 columnas**.

### Grouped analysis

The analysis accepts matrices with eight or nine columns. It reads only rows where column 8 is greater than `1`.

- It takes the value from column 4 (`D`).
- It separates values by column 3 (`C`): `C = 0` and `C = 1`.
- Each sequence is divided into three contiguous groups. If the count is not divisible by three, the extra values go to the first group and then the second group.
- The result window displays the detailed values and each group average. Missing groups are shown as `n/a`.

Column numbering in this document is one-based, matching the table seen by users.

## Packaging and file association

The macOS package uses `CFBundleDocumentTypes` in `packaging/macos/Info.plist` to declare `.mat` as a document type. This lets Finder offer MatVisor as an application for MATLAB matrix files after installation. Windows file association is intentionally left to the installer or the user's operating-system settings.

## Change guidelines

- Keep processing logic independent from Avalonia controls when adding new transformations or analyses.
- Do not alter the source `.mat` file during conversion; derived matrices must be saved to a user-selected destination.
- Add any new user-visible text to `AppLocalizer` in both languages instead of hard-coding it in a view.
- Update this document when a user-visible rule, file format assumption, or package behavior changes.
