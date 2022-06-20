using System;
using System.Collections.Generic;
using System.Linq;

namespace CPP.API.Extensions
{
    public static class IEnumerableExtensions
    {
        public static double GetStdDev(this IEnumerable<double> sample)
        {
            int n = sample.Count();
            if (n == 0) return 0.0;

            double mean = sample.Average();
            double variance = sample.Sum(x => (x - mean) * (x - mean)) / n;
            double stdDev = Math.Sqrt(variance);

            return stdDev;
        }

        public static double GetRSquare(this IEnumerable<double> sample1, IEnumerable<double> sample2)
        {
            double mean = sample1.Average();
            double sumOfSquaredTotal = sample1.Sum(x => Math.Pow(x - mean, 2));
            double sumOfSquaredRegression = sample1.Zip(sample2, (x1, x2) => Math.Pow(x1 - x2, 2)).Sum();
            return 1 - sumOfSquaredRegression / sumOfSquaredTotal;
        }

        public static double GetMAE(this IEnumerable<double> sample1, IEnumerable<double> sample2)
        {
            return sample1.Zip(sample2, (x1, x2) => Math.Abs(x1 - x2)).Average();
        }

        public static double GetMSE(this IEnumerable<double> sample1, IEnumerable<double> sample2)
        {
            return sample1.Zip(sample2, (x1, x2) => Math.Pow(x1 - x2, 2)).Average();
        }

        public static double GetMAPE(this IEnumerable<double> sample1, IEnumerable<double> sample2)
        {
            return sample1.Zip(sample2, (x1, x2) => Math.Abs((x1 - x2) / x1)).Average();
        }
        public static IEnumerable<string> SelectOrderedUniqueStrings<T>(this IEnumerable<T> collection, Func<T, string> getStringField)
        {
            return collection.Select(getStringField).Distinct().OrderBy(s => s);
        }
    }
}