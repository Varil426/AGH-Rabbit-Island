using System;
using System.Collections.Generic;
using System.Text;

namespace Rabbit_Island
{
    internal class Config
    {
        // TODO Add check for invalid values (or too big/small)
        public interface ICreatureConfig<Creature> where Creature : Entities.Creature
        {
            public Type CreatureType { get; set; }

            public int InitialPopulation { get; set; }

            public int MinChildren { get; set; }

            public int MaxChildren { get; set; }

            public int PregnancyDuration { get; set; }

            public int LifeExpectancy { get; set; }
        }

        private class CreatureConfig<Creature> : ICreatureConfig<Creature> where Creature : Entities.Creature
        {
            public CreatureConfig()
            {
                CreatureType = typeof(Creature);
            }

            public Type CreatureType { get; set; }

            public int InitialPopulation { get; set; }

            public int MinChildren { get; set; }

            public int MaxChildren { get; set; }
            public int PregnancyDuration { get; set; }

            public int LifeExpectancy { get; set; }
        }

        public Config()
        {
            RabbitConfig = new CreatureConfig<Entities.Rabbit>();
            WolvesConfig = new CreatureConfig<Entities.Wolf>();
        }

        private (int, int) _mapSize;

        public double TimeRate { get; set; }

        public bool DeathFromOldAge { get; set; }

        public int MaxCreatures { get; set; }

        private double _mutationChance;

        public double MutationChance
        {
            get => _mutationChance;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException("Value must be between 0 and 1");
                }
                _mutationChance = value;
            }
        }

        private double _mutationImpact;

        public double MutationImpact
        {
            get => _mutationImpact;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Value must be greater than or equal to 0");
                }
                _mutationImpact = value;
            }
        }

        public int FruitsPerDay { get; set; }

        public bool FoodExpires { get; set; }

        public bool DrawRanges { get; set; }

        public bool ExportResultsToCSV { get; set; }

        public World.GenerateOffspringMethod? SelectedOffspringGenerationMethod { get; set; }

        public (int, int) MapSize
        {
            get => _mapSize;

            set
            {
                if (value.Item1 <= 0 || value.Item2 <= 0)
                {
                    throw new ArgumentException("Map size should be greater than 0");
                }

                _mapSize = value;
            }
        }

        public ICreatureConfig<Entities.Rabbit> RabbitConfig { get; }

        public ICreatureConfig<Entities.Wolf> WolvesConfig { get; }
    }
}