﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rabbit_Island
{
    internal class Config
    {
        public interface ICreatureConfig<Creature> where Creature : Entities.Creature
        {
            public Type CreatureType { get; set; }

            public int InitialPopulation { get; set; }

            public int MinChildren { get; set; }

            public int MaxChildren { get; set; }
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
        }

        public Config()
        {
            RabbitConfig = new CreatureConfig<Entities.Rabbit>();
            WolvesConfig = new CreatureConfig<Entities.Wolf>();
        }

        public double TimeRate { get; set; }

        public bool? DeathFromOldAge { get; set; }

        public int MaxCreatures { get; set; }

        public int FruitsPerDay { get; set; }

        public int PregnancyDuration { get; set; }

        public bool? FoodExpires { get; set; }

        public bool? DrawRanges { get; set; }

        public (int, int) MapSize { get; set; }

        public ICreatureConfig<Entities.Rabbit> RabbitConfig { get; }

        public ICreatureConfig<Entities.Wolf> WolvesConfig { get; }
    }
}