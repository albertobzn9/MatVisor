using MathNet.Numerics.Data.Matlab;
using MathNet.Numerics.LinearAlgebra;
using System.IO;

namespace VisorMat;

internal static class MatFileLoader
{
    public static Matrix<double> LoadFirstNumericMatrix(string filePath)
    {
        foreach (var item in MatlabReader.List(filePath))
        {
            if (item.Name.StartsWith("__", StringComparison.Ordinal))
            {
                continue;
            }

            try
            {
                return MatlabReader.Unpack<double>(item);
            }
            catch (NotSupportedException)
            {
                // Otra variable del archivo puede ser una estructura, celda o arreglo no compatible.
            }
            catch (ArgumentException)
            {
                // Una matriz compleja no se puede mostrar como una tabla de valores double.
            }
        }

        throw new InvalidDataException("No se encontró una matriz numérica bidimensional válida en el archivo.");
    }
}
