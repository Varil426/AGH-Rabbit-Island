using Rabbit_Island.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit_Island
{
    internal class OffspringGeneration
    {
        private static Vector2 GenerateNearbyLocation(Creature creature)
        {
            var random = StaticRandom.Generator;
            var location = new Vector2
            {
                X = random.Next((int)creature.Position.X - 5, (int)creature.Position.X + 5),
                Y = random.Next((int)creature.Position.Y - 5, (int)creature.Position.Y + 5)
            };
            return location;
        }

        /// <summary>
        /// Generates offspring identical to one of the parents near the mother.
        /// </summary>
        /// <param name="mother">Mother creature.</param>
        /// <param name="father">Father creature.</param>
        /// <returns></returns>
        public static List<Creature> BasicOffspringGeneration(Creature mother, Creature father)
        {
            var offspring = new List<Creature>();
            var nearbyLocation = GenerateNearbyLocation(mother);
            var generation = Math.Max(mother.Generation, father.Generation) + 1;
            if (mother is Rabbit rabbitMother && father is Rabbit rabbitFather)
            {
                var offspringNumber = StaticRandom.Generator.Next(World.Instance.WorldConfig.RabbitConfig.MinChildren, World.Instance.WorldConfig.RabbitConfig.MaxChildren);
                for (int i = 0; i < offspringNumber; i++)
                {
                    Rabbit chosenParent;
                    if (StaticRandom.Generator.Next(2) == 0)
                    {
                        chosenParent = rabbitMother;
                    }
                    else
                    {
                        chosenParent = rabbitFather;
                    }
                    var child = new Rabbit(nearbyLocation, generation, chosenParent.MaxHealth, chosenParent.MaxEnergy, chosenParent.SightRange, chosenParent.MovementSpeed, chosenParent.InteractionRange, chosenParent.Fear);
                    offspring.Add(child);
                }
            }
            else if (mother is Wolf wolfMother && father is Wolf wolfFather)
            {
                var offspringNumber = StaticRandom.Generator.Next(World.Instance.WorldConfig.WolvesConfig.MinChildren, World.Instance.WorldConfig.WolvesConfig.MaxChildren);
                for (int i = 0; i < offspringNumber; i++)
                {
                    Wolf chosenParent;
                    if (StaticRandom.Generator.Next(2) == 0)
                    {
                        chosenParent = wolfMother;
                    }
                    else
                    {
                        chosenParent = wolfFather;
                    }
                    var child = new Wolf(nearbyLocation, generation, chosenParent.MaxHealth, chosenParent.MaxEnergy, chosenParent.SightRange, chosenParent.MovementSpeed, chosenParent.InteractionRange, chosenParent.Attack);
                    offspring.Add(child);
                }
            }
            return offspring;
        }
    }
}