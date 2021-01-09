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
                        // Remove dead creatures
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
                        var newFruit = new Fruit(StaticRandom.GenerateRandomPosition());
                        world.AddEntity(newFruit);
                    }
                    Thread.Sleep(dayDuration);
                }
            }
        }
    }
}