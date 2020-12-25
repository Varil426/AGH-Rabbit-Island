using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rabbit_Island
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private World world = World.Instance;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumericTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private Config CreateConfigFromUserInput()
        {
            var rabbitsInitialPopulation = int.Parse(RabbitsInitialPopulationInput.Text);
            var rabbitsMinChildren = int.Parse(RabbitsMinChildrenInput.Text);
            var rabbitsMaxChildren = int.Parse(RabbitsMaxChildrenInput.Text);

            var wolvesInitialPopulation = int.Parse(WolvesInitialPopulationInput.Text);
            var wolvesMinChildren = int.Parse(WolvesMinChildrenInput.Text);
            var wolvesMaxChildren = int.Parse(WolvesMaxChildrenInput.Text);

            var timeRate = double.Parse(TimeRateInput.Text);
            var deathFromOldAge = DeathFromOldAgeInput.IsChecked;
            var maxCreatures = int.Parse(MaxCreaturesInput.Text);
            var fruitsPerDay = int.Parse(FruitsPerDayInput.Text);
            var mapSize = int.Parse(MapSizeInput.Text);
            var pregnancyDuration = int.Parse(PregnancyDurationInput.Text);
            var drawRanges = DrawRangesInput.IsChecked;
            var config = new Config()
            {
                TimeRate = timeRate,
                DeathFromOldAge = deathFromOldAge,
                MaxCreatures = maxCreatures,
                FruitsPerDay = fruitsPerDay,
                PregnancyDuration = pregnancyDuration,
                DrawRanges = drawRanges,
                MapSize = (mapSize, mapSize)
            };

            config.RabbitConfig.InitialPopulation = rabbitsInitialPopulation;
            config.RabbitConfig.MinChildren = rabbitsMinChildren;
            config.RabbitConfig.MaxChildren = rabbitsMaxChildren;

            config.WolvesConfig.InitialPopulation = wolvesInitialPopulation;
            config.WolvesConfig.MinChildren = wolvesMinChildren;
            config.WolvesConfig.MaxChildren = wolvesMaxChildren;

            return config;
        }

        private void RunSimulation(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            world.WorldConfig = CreateConfigFromUserInput();
            world.WorldMap = new Map(world.WorldConfig.MapSize);
        }
    }
}