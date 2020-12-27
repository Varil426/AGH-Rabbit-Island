using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rabbit_Island.Entities
{
    internal class Fruit : Entity
    {
        public Fruit(float x, float y) : base(x, y)
        {
        }

        public override void DrawSelf(Canvas canvas)
        {
            var fruitCanvas = new Canvas();
            var rectangle = new Rectangle()
            {
                Width = 1,
                Height = 1,
                Fill = Brushes.White
            };
            fruitCanvas.Children.Add(rectangle);
            canvas.Children.Add(fruitCanvas);
            Canvas.SetLeft(fruitCanvas, Position.X);
            Canvas.SetTop(fruitCanvas, Position.Y);
        }
    }
}