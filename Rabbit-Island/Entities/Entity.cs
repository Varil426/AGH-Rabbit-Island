using System;
using System.Numerics;
using System.Windows.Controls;

namespace Rabbit_Island.Entities
{
    internal abstract class Entity : IWPFDrawable
    {
        protected Entity(float x, float y)
        {
            _position = new Vector2(x, y);
            CreateAt = DateTime.Now;
        }

        // TODO Maybe change Vector2 to something (or implement something) that uses double for greater precision
        private Vector2 _position;

        public Vector2 Position
        {
            get => new Vector2(_position.X, _position.Y);
            protected set => _position = value;
        }

        public DateTime CreateAt { get; }

        public bool IsHidden { get; set; }

        public abstract void DrawSelf(Canvas canvas);
    }
}