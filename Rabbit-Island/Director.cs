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

        private Thread? _directorThread;
        private bool _shouldRun;

        public void Stop()
        {
            _shouldRun = false;
            _directorThread?.Interrupt();
        }

        public void Start()
        {
            _directorThread = new Thread(Instance.Run)
            {
                Name = "Director Thread",
                IsBackground = true
            };
            _directorThread.Start();
        }

        private void Run()
        {
            lock (this)
            {
                int dayDuration = (int)((60 * 60 * 24 * 1000) / world.WorldConfig.TimeRate);
                int expirencyTime = dayDuration * 2;
                int fruitsPerDay = world.WorldConfig.FruitsPerDay;
                bool foodExpires = world.WorldConfig.FoodExpires;
                _shouldRun = true;
                try
                {
                    while (_shouldRun)
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
                                if (entity is Creature creature && !creature.IsAlive && creature.DeathAt.AddMilliseconds(expirencyTime) <= DateTime.Now)
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
                catch (ThreadInterruptedException)
                {
                }
            }
        }
    }
}