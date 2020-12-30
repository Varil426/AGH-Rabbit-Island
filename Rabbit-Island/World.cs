using Rabbit_Island.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace Rabbit_Island
{
    internal class World
    {
        private static readonly World _instance = new World();

        private readonly List<Entity> _entities;

        private Map _worldMap;

        public Config WorldConfig { get; set; }

        static World()
        {
        }

        private World()
        {
            _entities = new List<Entity>();
            _worldMap = new Map((1000, 1000));
            WorldConfig = new Config();
        }

        public static World Instance
        {
            get
            {
                return _instance;
            }
        }

        public void AddEntity(Entity entity)
        {
            lock (_entities)
            {
                _entities.Add(entity);
            }
        }

        public void RemoveEntity(Entity entity)
        {
            lock (_entities)
            {
                _entities.Remove(entity);
            }
        }

        public void StartSimulation()
        {
            // TODO Move it to fields
            var threads = new List<Thread>();
            foreach (ICreature creature in _entities.Where(x => x is ICreature))
            {
                var th = new Thread(creature.Act)
                {
                    IsBackground = true
                };
                threads.Add(th);
            }
            threads.ForEach(x => x.Start());
        }

        public List<Entity> GetCloseByEntities(Entity entity)
        {
            return _entities.Where(x =>
            {
                var distance = Vector2.Distance(entity.Position, x.Position);
                return distance <= 10;
            }).ToList();
        }

        public List<Entity> GetCloseByEntities(Creature creature)
        {
            return _entities.Where(x =>
            {
                var distance = Vector2.Distance(creature.Position, x.Position);
                return distance <= creature.SightRange;
            }).ToList();
        }

        public Map WorldMap
        {
            get => _worldMap;
            set => _worldMap = value;
        }

        public List<Entity> Entities => _entities;
    }
}