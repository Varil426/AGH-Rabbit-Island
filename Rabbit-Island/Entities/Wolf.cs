using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            throw new NotImplementedException();
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
            Canvas.SetLeft(wolfCanvas, Position.Item1);
            Canvas.SetTop(wolfCanvas, Position.Item2);
        }
    }
}