using System;
using System.Threading;

namespace Rabbit_Island
{
    internal static class StaticRandom
    {
        // TODO Change all random to use this

        private static int seed = Environment.TickCount;

        private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random(seed));

        public static Random Generator => _random.Value!;
    }
}