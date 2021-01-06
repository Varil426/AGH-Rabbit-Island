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
            var random = new Random();
            var location = new Vector2();
            location.X = random.Next((int)creature.Position.X - 5, (int)creature.Position.X + 5);
            location.Y = random.Next((int)creature.Position.Y - 5, (int)creature.Position.Y + 5);
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
            var random = new Random();
            if (mother is Rabbit rabbitMother && father is Rabbit rabbitFather)
            {
                var offspringNumber = random.Next(World.Instance.WorldConfig.RabbitConfig.MaxChildren);
                for (int i = 0; i < offspringNumber; i++)
                {
                    var nearbyLocation = GenerateNearbyLocation(mother);
                    Rabbit chosenParent;
                    if (random.Next(2) == 0)
                    {
                        chosenParent = rabbitMother;
                    }
                    else
                    {
                        chosenParent = rabbitFather;
                    }
                    var child = new Rabbit(nearbyLocation, chosenParent.MaxHealth, chosenParent.MaxEnergy, chosenParent.SightRange, chosenParent.MovementSpeed, chosenParent.InteractionRange, chosenParent.Fear);
                    offspring.Add(child);
                }
            }
            else if (mother is Wolf wolfMother && father is Wolf wolfFather)
            {
                // TODO
            }
            return offspring;
        }
    }
}