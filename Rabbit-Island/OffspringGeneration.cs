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
            if (mother.Gender != Creature.GenderType.Female || father.Gender != Creature.GenderType.Male)
            {
                throw new ArgumentException("Invalid creatures passed as arguments");
            }
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

        private static double Mutate(double value)
        {
            if (StaticRandom.Generator.Next(100) >= 90)
            {
                value = StaticRandom.Generator.Next((int)(value - value / 10), (int)(value + value / 10));
            }
            return value;
        }

        public static List<Creature> OffspringGenerationWithMutations(Creature mother, Creature father)
        {
            var offspring = new List<Creature>();
            var nearbyLocation = GenerateNearbyLocation(mother);
            var generation = Math.Max(mother.Generation, father.Generation) + 1;

            var traits = new Dictionary<string, double>
            {
                { "MaxHealth", 0 },
                { "MaxEnergy", 0 },
                { "SightRange", 0 },
                { "MovementSpeed", 0 },
                { "InteractionRange", 0 }
            };

            if (mother is Rabbit rabbitMother && father is Rabbit rabbitFather)
            {
                traits.Add("Fear", 0);
                var offspringNumber = StaticRandom.Generator.Next(World.Instance.WorldConfig.RabbitConfig.MinChildren, World.Instance.WorldConfig.RabbitConfig.MaxChildren);
                for (int i = 0; i < offspringNumber; i++)
                {
                    if (StaticRandom.Generator.Next(2) == 0)
                        traits["MaxHealth"] = rabbitMother.MaxHealth;
                    else
                        traits["MaxHealth"] = rabbitFather.MaxHealth;
                    traits["MaxHealth"] = Mutate(traits["MaxHealth"]);

                    if (StaticRandom.Generator.Next(2) == 0)
                        traits["MaxEnergy"] = rabbitMother.MaxEnergy;
                    else
                        traits["MaxEnergy"] = rabbitFather.MaxEnergy;
                    traits["MaxEnergy"] = Mutate(traits["MaxEnergy"]);

                    if (StaticRandom.Generator.Next(2) == 0)
                        traits["SightRange"] = rabbitMother.SightRange;
                    else
                        traits["SightRange"] = rabbitFather.SightRange;
                    traits["SightRange"] = Mutate(traits["SightRange"]);

                    if (StaticRandom.Generator.Next(2) == 0)
                        traits["MovementSpeed"] = rabbitMother.MovementSpeed;
                    else
                        traits["MovementSpeed"] = rabbitFather.MovementSpeed;
                    traits["MovementSpeed"] = Mutate(traits["MovementSpeed"]);

                    if (StaticRandom.Generator.Next(2) == 0)
                        traits["InteractionRange"] = rabbitMother.InteractionRange;
                    else
                        traits["InteractionRange"] = rabbitFather.InteractionRange;
                    traits["InteractionRange"] = Mutate(traits["InteractionRange"]);

                    if (StaticRandom.Generator.Next(2) == 0)
                        traits["Fear"] = rabbitMother.Fear;
                    else
                        traits["Fear"] = rabbitFather.Fear;
                    traits["Fear"] = Mutate(traits["Fear"]);

                    var child = new Rabbit(nearbyLocation, generation, traits["MaxHealth"], traits["MaxEnergy"], traits["SightRange"], traits["MovementSpeed"], traits["InteractionRange"], (int)traits["Fear"]);
                    offspring.Add(child);
                }
            }
            else if (mother is Wolf wolfMother && father is Wolf wolfFather)
            {
                traits.Add("Attack", 0);
                var offspringNumber = StaticRandom.Generator.Next(World.Instance.WorldConfig.WolvesConfig.MinChildren, World.Instance.WorldConfig.WolvesConfig.MaxChildren);
                for (int i = 0; i < offspringNumber; i++)
                {
                    if (StaticRandom.Generator.Next(2) == 0)
                        traits["MaxHealth"] = wolfMother.MaxHealth;
                    else
                        traits["MaxHealth"] = wolfFather.MaxHealth;
                    traits["MaxHealth"] = Mutate(traits["MaxHealth"]);

                    if (StaticRandom.Generator.Next(2) == 0)
                        traits["MaxEnergy"] = wolfMother.MaxEnergy;
                    else
                        traits["MaxEnergy"] = wolfFather.MaxEnergy;
                    traits["MaxEnergy"] = Mutate(traits["MaxEnergy"]);

                    if (StaticRandom.Generator.Next(2) == 0)
                        traits["SightRange"] = wolfMother.SightRange;
                    else
                        traits["SightRange"] = wolfFather.SightRange;
                    traits["SightRange"] = Mutate(traits["SightRange"]);

                    if (StaticRandom.Generator.Next(2) == 0)
                        traits["MovementSpeed"] = wolfMother.MovementSpeed;
                    else
                        traits["MovementSpeed"] = wolfFather.MovementSpeed;
                    traits["MovementSpeed"] = Mutate(traits["MovementSpeed"]);

                    if (StaticRandom.Generator.Next(2) == 0)
                        traits["InteractionRange"] = wolfMother.InteractionRange;
                    else
                        traits["InteractionRange"] = wolfFather.InteractionRange;
                    traits["InteractionRange"] = Mutate(traits["InteractionRange"]);

                    if (StaticRandom.Generator.Next(2) == 0)
                        traits["Attack"] = wolfMother.Attack;
                    else
                        traits["Attack"] = wolfFather.Attack;
                    traits["Attack"] = Mutate(traits["Attack"]);

                    var child = new Wolf(nearbyLocation, generation, traits["MaxHealth"], traits["MaxEnergy"], traits["SightRange"], traits["MovementSpeed"], traits["InteractionRange"], (int)traits["Attack"]);
                    offspring.Add(child);
                }
            }

            return offspring;
        }
    }
}