using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Rabbit_Island.Entities;

namespace Rabbit_Island
{
    /// <summary>
    /// Interaction logic for GraphsWindow.xaml
    /// </summary>
    public partial class GraphsWindow : Window
    {
        private Thread _thread;

        private bool _threadRun;

        private string GenerateTimeString(double simulationTimeMinutes)
        {
            var minutes = (int)(simulationTimeMinutes % 60);
            var hours = (int)(simulationTimeMinutes % (60 * 24) / 60);
            var days = (int)(simulationTimeMinutes / (60 * 24));
            return $"Simulation Time: Days: {days}, Hours: {hours}, Minutes: {minutes}";
        }

        private void RunUpdateStatus()
        {
            var timeout = 1000 / 5;
            var world = World.Instance;
            double simulationTimeMinutes;
            _threadRun = true;
            while (_threadRun)
            {
                simulationTimeMinutes = (DateTime.Now - world.StartTime).TotalMinutes * world.WorldConfig.TimeRate;
                try
                {
                    Dispatcher.Invoke(() => SimulationTimeText.Text = GenerateTimeString(simulationTimeMinutes));
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {
                }

                _rabbitsData.Points.Add(new DataPoint(simulationTimeMinutes, world.GetAllEntities().OfType<Rabbit>().Where(rabbit => rabbit.IsAlive).Count()));
                _wolvesData.Points.Add(new DataPoint(simulationTimeMinutes, world.GetAllEntities().OfType<Wolf>().Where(wolf => wolf.IsAlive).Count()));
                RabbitsPlot.InvalidatePlot(true);
                WolvesPlot.InvalidatePlot(true);
                Thread.Sleep(timeout);
            }
        }

        private (Axis, Axis) GenerateAxesFor(string name)
        {
            var xAxis = new LinearAxis();
            xAxis.Position = AxisPosition.Bottom;
            xAxis.Minimum = 0;
            xAxis.Maximum = 5000;
            xAxis.Title = "Time [minutes in simulation]";

            var yAxis = new LinearAxis();
            yAxis.Position = AxisPosition.Left;
            yAxis.Minimum = 0;
            yAxis.Maximum = 50; //World.Instance.WorldConfig.MaxCreatures;
            yAxis.Title = name;

            return (xAxis, yAxis);
        }

        public GraphsWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            _rabbitsData = new LineSeries();
            _wolvesData = new LineSeries();

            RabbitsPlot = new PlotModel
            {
                Title = "Rabbits",
            };

            var rabbitAxes = GenerateAxesFor("Rabbits");
            RabbitsPlot.Axes.Add(rabbitAxes.Item1);
            RabbitsPlot.Axes.Add(rabbitAxes.Item2);

            RabbitsPlot.Series.Add(_rabbitsData);

            WolvesPlot = new PlotModel
            {
                Title = "Wolves"
            };

            var wolvesAxes = GenerateAxesFor("Wolves");
            WolvesPlot.Axes.Add(wolvesAxes.Item1);
            WolvesPlot.Axes.Add(wolvesAxes.Item2);

            WolvesPlot.Series.Add(_wolvesData);

            _thread = new Thread(RunUpdateStatus)
            {
                IsBackground = true
            };
            _thread.Start();
        }

        private readonly LineSeries _rabbitsData;

        private readonly LineSeries _wolvesData;

        public PlotModel RabbitsPlot { get; private set; }

        public PlotModel WolvesPlot { get; private set; }

        public void StopAndClose()
        {
            Close();
            _threadRun = false;
        }
    }
}