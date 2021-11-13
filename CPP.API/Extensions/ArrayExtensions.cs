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
    }
}