using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rabbit_Island.Entities
{
    internal class Point : Entity
    {
        public Point(float x, float y) : base(x, y)
        {
        }

        public Point(Vector2 destination) : base(destination)
        {
        }

        public override void DrawSelf(Canvas canvas)
        {
            // TODO Implement maybe?
            throw new NotImplementedException();
        }
    }
}