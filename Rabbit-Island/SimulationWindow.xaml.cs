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
        private readonly Thread _thread;

        private bool _threadRun;

        private readonly World world = World.Instance;

        private readonly Canvas canvas;

        public SimulationWindow()
        {
            InitializeComponent();
            canvas = new Canvas
            {
                Width = world.WorldMap.Size.Item1,
                Height = world.WorldMap.Size.Item2,
                Background = Brushes.Green
            };
            Plane.Children.Add(canvas);

            _thread = new Thread(DrawSimulation)
            {
                IsBackground = true
            };
            _thread.Start();
        }

        private void DrawSimulation()
        {
            var timeout = 1000 / 30;
            _threadRun = true;
            while (_threadRun)
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        canvas.Children.Clear();

                        foreach (var entity in world.GetAllEntities())
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

        public void StopAndClose()
        {
            Close();
            _threadRun = false;
        }
    }
}