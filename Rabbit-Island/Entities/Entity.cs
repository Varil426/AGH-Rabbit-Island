using System;
using System.Windows.Controls;

namespace Rabbit_Island.Entities
{
    internal abstract class Entity : IWPFDrawable
    {
        protected Entity(float x, float y)
        {
            Position = (x, y);
            CreateAt = DateTime.Now;
        }

        public (float, float) Position { get; }

        public DateTime CreateAt { get; }

        public bool IsHidden { get; set; }

        public abstract void DrawSelf(Canvas canvas);
    }
}