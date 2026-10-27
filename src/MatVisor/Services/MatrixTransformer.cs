using MathNet.Numerics.LinearAlgebra;
using Avalonia;

namespace MatVisor;

internal static class MatrixTransformer
{
    public static Matrix<double> CreateEightColumnView(Matrix<double> originalMatrix, out int removedRowCount)
    {
        if (originalMatrix.ColumnCount != 9)
        {
            throw new ArgumentException(
                ((App)Application.Current!).Localizer.Get(UiText.ConversionRequiresNineColumns),
                nameof(originalMatrix));
        }

        var remainingRows = new List<int>();
        for (var row = 0; row < originalMatrix.RowCount; row++)
        {
            if (originalMatrix[row, 8] != 2)
            {
                remainingRows.Add(row);
            }
        }

        removedRowCount = originalMatrix.RowCount - remainingRows.Count;
        return Matrix<double>.Build.Dense(
            remainingRows.Count,
            8,
            (row, column) => column == 0
                ? row + 1
                : originalMatrix[remainingRows[row], column]);
    }
}
