using System;
using System.Collections.Generic;
using System.Linq;

namespace CPP.API.Extensions
{
    public static class IEnumerableExtensions
    {
        public static double StdDev(this IEnumerable<double> values)
        {
            int n = values.Count();
            if (n == 0) return 0.0;

            double mean = values.Average();
            double variance = values.Sum(x => (x - mean) * (x - mean)) / n;
            double stdDev = Math.Sqrt(variance);

            return stdDev;
        }
    }
}