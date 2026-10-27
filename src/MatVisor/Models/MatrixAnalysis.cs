using MathNet.Numerics.LinearAlgebra;
using Avalonia;

namespace MatVisor;

internal sealed record MatrixAnalysis(
    Matrix<double> DisplayMatrix,
    int CeroCount,
    int UnoCount,
    IReadOnlyList<double> CeroAverages,
    IReadOnlyList<double> UnoAverages,
    IReadOnlyDictionary<(int Row, int Column), string> DisplayValues,
    IReadOnlySet<(int Row, int Column)> BoldCells);

internal static class MatrixAnalysisBuilder
{
    public static MatrixAnalysis Create(Matrix<double> source)
    {
        if (source.ColumnCount is not (8 or 9))
        {
            throw new ArgumentException(((App)Application.Current!).Localizer.Get(UiText.AnalysisRequiresEightOrNineColumns), nameof(source));
        }

        var valuesWithCero = new List<double>();
        var valuesWithUno = new List<double>();

        for (var row = 0; row < source.RowCount; row++)
        {
            // Los índices son cero-basados: C = 2, D = 3 y la columna 8 = 7.
            if (source[row, 7] <= 1)
            {
                continue;
            }

            if (source[row, 2] == 0)
            {
                valuesWithCero.Add(source[row, 3]);
            }
            else if (source[row, 2] == 1)
            {
                valuesWithUno.Add(source[row, 3]);
            }
        }

        var ceroGroups = SplitInThree(valuesWithCero);
        var unoGroups = SplitInThree(valuesWithUno);
        var ceroDetail = BuildDetailColumn(ceroGroups);
        var unoDetail = BuildDetailColumn(unoGroups);
        var rowCount = Math.Max(3, Math.Max(ceroDetail.Count, unoDetail.Count));
        var output = Matrix<double>.Build.Dense(rowCount, 4, double.NaN);
        var displayValues = new Dictionary<(int Row, int Column), string>();
        var boldCells = new HashSet<(int Row, int Column)>();

        CopyDetailColumn(ceroDetail, output, 0, displayValues, boldCells);
        CopyDetailColumn(unoDetail, output, 1, displayValues, boldCells);

        for (var group = 0; group < 3; group++)
        {
            var ceroAverage = Average(ceroGroups[group]);
            var unoAverage = Average(unoGroups[group]);
            output[group, 2] = ceroAverage;
            output[group, 3] = unoAverage;
            boldCells.Add((group, 2));
            boldCells.Add((group, 3));

            if (double.IsNaN(ceroAverage))
            {
                displayValues[(group, 2)] = "n/a";
            }

            if (double.IsNaN(unoAverage))
            {
                displayValues[(group, 3)] = "n/a";
            }
        }

        return new MatrixAnalysis(
            output,
            valuesWithCero.Count,
            valuesWithUno.Count,
            ceroGroups.Select(Average).ToArray(),
            unoGroups.Select(Average).ToArray(),
            displayValues,
            boldCells);
    }

    private static IReadOnlyList<List<double>> SplitInThree(IReadOnlyList<double> values)
    {
        var baseSize = values.Count / 3;
        var remainder = values.Count % 3;
        var groups = new List<List<double>>(3);
        var nextValue = 0;

        for (var group = 0; group < 3; group++)
        {
            var groupSize = baseSize + (group < remainder ? 1 : 0);
            var valuesInGroup = values.Skip(nextValue).Take(groupSize).ToList();
            groups.Add(valuesInGroup);
            nextValue += groupSize;
        }

        return groups;
    }

    private static List<DetailValue> BuildDetailColumn(IReadOnlyList<List<double>> groups)
    {
        var detail = new List<DetailValue>();

        for (var group = 0; group < groups.Count; group++)
        {
            foreach (var value in groups[group])
            {
                detail.Add(new DetailValue(value));
            }

            var average = Average(groups[group]);
            detail.Add(new DetailValue(average, IsAverage: true, IsNotApplicable: double.IsNaN(average)));

            // La fila vacía conserva la separación entre los tres conjuntos del ejemplo.
            if (group < groups.Count - 1)
            {
                detail.Add(new DetailValue(double.NaN));
            }
        }

        return detail;
    }

    private static void CopyDetailColumn(
        IReadOnlyList<DetailValue> detail,
        Matrix<double> output,
        int column,
        IDictionary<(int Row, int Column), string> displayValues,
        ISet<(int Row, int Column)> boldCells)
    {
        for (var row = 0; row < detail.Count; row++)
        {
            output[row, column] = detail[row].Value;
            if (detail[row].IsNotApplicable)
            {
                displayValues[(row, column)] = "n/a";
            }

            if (detail[row].IsAverage)
            {
                boldCells.Add((row, column));
            }
        }
    }

    private static double Average(IReadOnlyList<double> values) => values.Count == 0 ? double.NaN : values.Average();

    private sealed record DetailValue(double Value, bool IsAverage = false, bool IsNotApplicable = false);
}
