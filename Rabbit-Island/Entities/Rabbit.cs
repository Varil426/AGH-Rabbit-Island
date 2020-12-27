using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
            // TODO Improve this
            while (true)
            {
                var destination = new Vector2(300, 300);
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
            var rabbitCanvas = new Canvas();
            var color = Brushes.Blue;
            var rectangle = new Rectangle()
            {
                Width = 1,
                Height = 1,
                Fill = color
            };
            rabbitCanvas.Children.Add(rectangle);
            if (World.Instance.WorldConfig.DrawRanges)
            {
                var sightRange = new Ellipse()
                {
                    Width = SightRange,
                    Height = SightRange,
                    Stroke = color
                };
                rabbitCanvas.Children.Add(sightRange);
                Canvas.SetLeft(sightRange, -SightRange / 2);
                Canvas.SetTop(sightRange, -SightRange / 2);
            }
            canvas.Children.Add(rabbitCanvas);
            Canvas.SetLeft(rabbitCanvas, Position.X);
            Canvas.SetTop(rabbitCanvas, Position.Y);
        }
    }
}