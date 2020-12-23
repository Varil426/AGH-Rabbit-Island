using System;
using System.Collections.Generic;
using System.Text;

namespace Rabbit_Island.Entities
{
    internal class Rabbit : Creature
    {
        public Rabbit(float x, float y) : base(x, y)
        {
            // TODO Dodać ustawianie fear
        }

        public int Fear { get; }

        public override void Act()
        {
            throw new NotImplementedException();
        }
    }
}