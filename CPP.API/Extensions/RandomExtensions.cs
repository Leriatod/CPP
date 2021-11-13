using System;

namespace CPP.API.Extensions
{
    public static class RandomExtensions
    {
        public static double NextDoubleBetween(this Random random, double min, double max)
        {
            return random.NextDouble() * (max - min) + min;
        }
    }
}