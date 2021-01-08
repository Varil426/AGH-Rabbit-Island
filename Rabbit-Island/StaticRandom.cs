using System;
using System.Numerics;
using System.Threading;

namespace Rabbit_Island
{
    /// <summary>
    /// Handles all things related to random generation.
    /// </summary>
    internal static class StaticRandom
    {
        private static int seed = Environment.TickCount;

        private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random(seed));

        public static Random Generator => _random.Value!;

        public static Vector2 GenerateRandomPosition()
        {
            float x = Generator.Next(World.Instance.WorldMap.Size.Item1);
            float y = Generator.Next(World.Instance.WorldMap.Size.Item2);
            return new Vector2(x, y);
        }
    }
}