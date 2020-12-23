using System;
using System.Collections.Generic;
using System.Text;

namespace Rabbit_Island
{
    internal class Map
    {
        public Map((int, int) size)
        {
            Size = size;
        }

        public (int, int) Size { get; }
    }
}