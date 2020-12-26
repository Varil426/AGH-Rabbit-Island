using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit_Island.Entities
{
    internal class Wolf : Creature
    {
        public Wolf(float x, float y) : base(x, y)
        {
            Random random = new Random();
            MaxHealth = random.Next(90, 110);
            MaxEnergy = random.Next(90, 110);
            SightRange = random.Next(50);
            MovementSpeed = random.Next(5, 20);
            InteractionRange = random.Next(10);
        }

        public override void Act()
        {
            throw new NotImplementedException();
        }
    }
}