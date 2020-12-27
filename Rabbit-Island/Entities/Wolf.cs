using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
            // TODO Improve this
            while (true)
            {
                var destination = new Vector2(100, 100);
                var distance = Vector2.Distance(Position, destination);
                Move(destination);
                // TODO Move this sleep or make dependent on something
                Thread.Sleep(50);
                if (distance < 5)
                    break;
            }
        }

        public override void DrawSelf(Canvas canvas)
        {
            var wolfCanvas = new Canvas();
            var color = Brushes.Red;
            var rectangle = new Rectangle()
            {
                Width = 1,
                Height = 1,
                Fill = color
            };
            wolfCanvas.Children.Add(rectangle);
            if (World.Instance.WorldConfig.DrawRanges)
            {
                var sightRange = new Ellipse()
                {
                    Width = SightRange,
                    Height = SightRange,
                    Stroke = color
                };
                wolfCanvas.Children.Add(sightRange);
                Canvas.SetLeft(sightRange, -SightRange / 2);
                Canvas.SetTop(sightRange, -SightRange / 2);
            }
            canvas.Children.Add(wolfCanvas);
            Canvas.SetLeft(wolfCanvas, Position.X);
            Canvas.SetTop(wolfCanvas, Position.Y);
        }
    }
}