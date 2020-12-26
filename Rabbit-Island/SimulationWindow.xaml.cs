using Rabbit_Island.Entities;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rabbit_Island
{
    /// <summary>
    /// Interaction logic for SimulationWindow.xaml
    /// </summary>
    public partial class SimulationWindow : Window
    {
        private World world = World.Instance;

        private Canvas canvas;

        public SimulationWindow()
        {
            InitializeComponent();
            canvas = new Canvas
            {
                Width = world.WorldMap.Size.Item1,
                Height = world.WorldMap.Size.Item2,
                Background = Brushes.Green
            };
            Content.Children.Add(canvas);

            var th = new Thread(DrawSimulation)
            {
                IsBackground = true
            };
            th.Start();
        }

        public void DrawSimulation()
        {
            var timeout = 1000 / 30;
            while (true)
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        canvas.Children.Clear();

                        foreach (var entity in world.Entities)
                        {
                            entity.DrawSelf(canvas);
                        }
                    });
                    Thread.Sleep(timeout);
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {
                }
            }
        }
    }
}