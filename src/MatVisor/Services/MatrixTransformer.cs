using MathNet.Numerics.LinearAlgebra;

namespace MatVisor;

internal static class MatrixTransformer
{
    public static Matrix<double> CreateEightColumnView(Matrix<double> originalMatrix, out int removedRowCount)
    {
        if (originalMatrix.ColumnCount != 9)
        {
            throw new ArgumentException("La conversión a 8 columnas requiere una matriz de exactamente 9 columnas.", nameof(originalMatrix));
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
