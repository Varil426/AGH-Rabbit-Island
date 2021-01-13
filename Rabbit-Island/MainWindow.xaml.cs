﻿using Rabbit_Island.Entities;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rabbit_Island
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private World world = World.Instance;

        private SimulationWindow? _simulationWindow;

        private GraphsWindow? _graphsWindow;

        // TODO Add check for invalid values (or too big/small)

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumericTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TimeRateChange(object sender, TextChangedEventArgs args)
        {
            Regex regex = new Regex("^0+$");
            if (TimeRateInput.Text.Length == 0 || regex.IsMatch(TimeRateInput.Text))
            {
                TimeRateLabel.Content = "Invalid Value!";
            }
            else
            {
                var timeRate = Double.Parse(TimeRateInput.Text);
                var newText = $"Time Rate ({3600 / timeRate} seconds = 1 real-time hour)";
                TimeRateLabel.Content = newText;
            }
        }

        private Config CreateConfigFromUserInput()
        {
            var rabbitsInitialPopulation = int.Parse(RabbitsInitialPopulationInput.Text);
            var rabbitsMinChildren = int.Parse(RabbitsMinChildrenInput.Text);
            var rabbitsMaxChildren = int.Parse(RabbitsMaxChildrenInput.Text);
            var rabbitsPregnancyDuration = int.Parse(RabbitsPregnancyDurationInput.Text);
            var rabbitsLifeExpectancy = int.Parse(RabbitsLifeExpectancy.Text);

            var wolvesInitialPopulation = int.Parse(WolvesInitialPopulationInput.Text);
            var wolvesMinChildren = int.Parse(WolvesMinChildrenInput.Text);
            var wolvesMaxChildren = int.Parse(WolvesMaxChildrenInput.Text);
            var wolvesPregnancyDuration = int.Parse(WolvesPregnancyDurationInput.Text);
            var wolvesLifeExpectancy = int.Parse(WolvesLifeExpectancy.Text);

            var timeRate = double.Parse(TimeRateInput.Text);
            var deathFromOldAge = (bool)DeathFromOldAgeInput.IsChecked!;
            var maxCreatures = int.Parse(MaxCreaturesInput.Text);
            var fruitsPerDay = int.Parse(FruitsPerDayInput.Text);
            var mapSize = int.Parse(MapSizeInput.Text);
            var drawRanges = (bool)DrawRangesInput.IsChecked!;

            World.GenerateOffspringMethod generateOffspringMethod;

            switch (OffspringGenerationMethodInput.SelectedIndex)
            {
                case 0:
                    generateOffspringMethod = OffspringGeneration.BasicOffspringGeneration;
                    break;

                default:
                    throw new ArgumentException("Invalid OffspringGenerationMethod selected");
            }

            var config = new Config()
            {
                TimeRate = timeRate,
                DeathFromOldAge = deathFromOldAge,
                MaxCreatures = maxCreatures,
                FruitsPerDay = fruitsPerDay,
                DrawRanges = drawRanges,
                MapSize = (mapSize, mapSize),
                SelectedOffspringGenerationMethod = generateOffspringMethod
            };

            config.RabbitConfig.InitialPopulation = rabbitsInitialPopulation;
            config.RabbitConfig.MinChildren = rabbitsMinChildren;
            config.RabbitConfig.MaxChildren = rabbitsMaxChildren;
            config.RabbitConfig.PregnancyDuration = rabbitsPregnancyDuration;
            config.RabbitConfig.LifeExpectancy = rabbitsLifeExpectancy;

            config.WolvesConfig.InitialPopulation = wolvesInitialPopulation;
            config.WolvesConfig.MinChildren = wolvesMinChildren;
            config.WolvesConfig.MaxChildren = wolvesMaxChildren;
            config.WolvesConfig.PregnancyDuration = wolvesPregnancyDuration;
            config.WolvesConfig.LifeExpectancy = wolvesLifeExpectancy;

            return config;
        }

        private void CreateInitialCreatures()
        {
            // Create Rabbits
            for (int i = 0; i < world.WorldConfig.RabbitConfig.InitialPopulation; i++)
            {
                world.AddEntity(new Rabbit(StaticRandom.GenerateRandomPosition()));
            }
            // Create Wolves
            for (int i = 0; i < world.WorldConfig.WolvesConfig.InitialPopulation; i++)
            {
                world.AddEntity(new Wolf(StaticRandom.GenerateRandomPosition()));
            }
        }

        private void StartSimulation(object sender, RoutedEventArgs e)
        {
            ConfigGrid.ColumnDefinitions[0].IsEnabled = false;
            ConfigGrid.ColumnDefinitions[1].IsEnabled = false;
            StartStopButton.Content = "Stop";
            StartStopButton.Click -= StartSimulation;
            StartStopButton.Click += StopSimulation;

            world.WorldConfig = CreateConfigFromUserInput();
            world.WorldMap = new Map(world.WorldConfig.MapSize);
            if (world.WorldConfig.SelectedOffspringGenerationMethod is World.GenerateOffspringMethod)
            {
                World.GenerateOffspring = world.WorldConfig.SelectedOffspringGenerationMethod;
            }
            // Scale values in simulation to TimeRate
            Rabbit.RaceValues.RefreshValues();
            Wolf.RaceValues.RefreshValues();
            CreateInitialCreatures();

            _simulationWindow = new SimulationWindow();
            _graphsWindow = new GraphsWindow();
            _graphsWindow.Show();
            _simulationWindow.Show();

            world.StartSimulation();
        }

        private void StopSimulation(object sender, RoutedEventArgs e)
        {
            ConfigGrid.ColumnDefinitions[0].IsEnabled = true;
            ConfigGrid.ColumnDefinitions[1].IsEnabled = true;
            StartStopButton.Content = "Run";
            StartStopButton.Click -= StopSimulation;
            StartStopButton.Click += StartSimulation;

            _graphsWindow?.StopAndClose();
            _simulationWindow?.StopAndClose();

            world.Reset();
        }
    }
}