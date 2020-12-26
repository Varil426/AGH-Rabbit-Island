using System;
using System.Collections.Generic;
using System.Text;

namespace Rabbit_Island.Entities
{
    internal abstract class Entity
    {
        protected Entity(float x, float y)
        {
            Position = (x, y);
            CreateAt = DateTime.Now;
        }

        public (float, float) Position { get; }

        public DateTime CreateAt { get; }

        public bool IsHidden { get; set; }
    }
}