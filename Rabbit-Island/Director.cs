using Rabbit_Island.Entities;
using System;
using System.Threading;

namespace Rabbit_Island
{
    /// <summary>
    /// Singleton that is meant to be managing not-alive entities.
    /// </summary>
    internal class Director
    {
        private readonly static Director _instance = new Director();

        public static Director Instance
        {
            get
            {
                return _instance;
            }
        }

        private readonly World world;

        static Director()
        {
        }

        private Director()
        {
            world = World.Instance;
        }

        private (float, float) GenerateRandomPosition()
        {
            Random random = new Random();
            float x = random.Next(world.WorldMap.Size.Item1);
            float y = random.Next(world.WorldMap.Size.Item2);
            return (x, y);
        }

        public void Run()
        {
            lock (this)
            {
                int dayDuration = (int)((60 * 60 * 24 * 1000) / world.WorldConfig.TimeRate);
                int expirencyTime = dayDuration * 2;
                int fruitsPerDay = world.WorldConfig.FruitsPerDay;
                bool foodExpires = world.WorldConfig.FoodExpires;
                while (true)
                {
                    if (foodExpires)
                    {
                        // Remove old fruits
                        world.GetAllEntities().FindAll(entity => entity is Fruit).ForEach(fruit =>
                        {
                            if (fruit.CreatedAt.AddMilliseconds(expirencyTime) <= DateTime.Now)
                            {
                                world.RemoveEntity(fruit);
                            }
                        });

                        world.GetAllEntities().FindAll(entity => entity is Creature).ForEach(entity =>
                        {
                            var creature = entity as Creature;
                            if (creature != null && !creature.IsAlive && creature.DeathAt.AddMilliseconds(expirencyTime) <= DateTime.Now)
                            {
                                world.RemoveEntity(creature);
                            }
                        });
                    }
                    // Add new fruits
                    for (int i = 0; i < fruitsPerDay; i++)
                    {
                        var randomPosition = GenerateRandomPosition();
                        var newFruit = new Fruit(randomPosition.Item1, randomPosition.Item2);
                        world.AddEntity(newFruit);
                    }
                    Thread.Sleep(dayDuration);
                }
            }
        }
    }
}