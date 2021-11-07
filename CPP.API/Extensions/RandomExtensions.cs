using System;

namespace CPP.API.Extensions
{
    public static class RandomExtensions
    {
        public static double NextDoubleBetween(this Random random, double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}