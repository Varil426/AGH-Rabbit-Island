using System;
using System.Collections.Generic;
using System.Text;

namespace Rabbit_Island
{
    internal class Map
    {
        public Map((int, int) size)
        {
            if (!CheckMapSize(size))
            {
                throw new ArgumentException("Map size should be greater than 0");
            }
            Size = size;
        }

        private bool CheckMapSize((int, int) size)
        {
            if (size.Item1 <= 0 || size.Item2 <= 0)
            {
                return false;
            }
            return true;
        }

        public Map Resize((int, int) size)
        {
            if (!CheckMapSize(size))
            {
                throw new ArgumentException("Map size should be greater than 0");
            }
            Size = size;
            return this;
        }

        public (int, int) Size { get; private set; }
    }
}