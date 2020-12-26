using System;
using System.Collections.Generic;
using System.Text;

namespace Rabbit_Island.Entities
{
    internal class Rabbit : Creature
    {
        public Rabbit(float x, float y) : base(x, y)
        {
            Random random = new Random();
            MaxHealth = random.Next(90, 110);
            MaxEnergy = random.Next(90, 110);
            SightRange = random.Next(50);
            MovementSpeed = random.Next(5, 20);
            InteractionRange = random.Next(10);
            Fear = random.Next(10);
        }

        public int Fear { get; }

        public override void Act()
        {
            throw new NotImplementedException();
        }
    }
}