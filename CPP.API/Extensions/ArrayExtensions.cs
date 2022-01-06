using System;
using System.Linq;

namespace CPP.API.Extensions
{
    public static class ArrayExtensions
    {
        public static T[][] RemoveLastColumn<T>(this T[][] data)
        {
            return data.Select(row => row.SkipLast(1).ToArray()).ToArray();
        }

        public static T[][] TakeLastColumns<T>(this T[][] data, int columnsNumberToTake)
        {
            return data.Select(row => row.Skip(row.Length - columnsNumberToTake).ToArray()).ToArray();
        }

        public static T[] RemoveLast<T>(this T[] array)
        {
            return array.Take(array.Length - 1).ToArray();
        }

        public static T[] Shuffle<T>(this T[] array)
        {
            return array.OrderBy(x => Guid.NewGuid()).ToArray();
        }
    }
}