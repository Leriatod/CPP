using System.Linq;

namespace CPP.API.Extensions
{
    public static class ArrayExtensions
    {
        public static double[][] RemoveLastColumn(this double[][] data)
        {
            return data.Select(c => c.SkipLast(1).ToArray()).ToArray();
        }

        public static double[] GetLastColumn(this double[][] data)
        {
            return data.Select(c => c.Last()).ToArray();
        }

        public static double[] RemoveLast(this double[] array)
        {
            return array.Take(array.Length - 1).ToArray();
        }
    }
}