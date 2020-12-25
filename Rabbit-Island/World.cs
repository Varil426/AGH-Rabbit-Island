using Rabbit_Island.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            _entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
        }

        public void StartSimulation()
        {
            throw new NotImplementedException();
        }

        public List<Entity> GetCloseByEntities(Entity entity)
        {
            return _entities.Where(x =>
            {
                var xDiff = x.Position.Item1 - entity.Position.Item1;
                var yDiff = x.Position.Item2 - entity.Position.Item2;

                return Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2)) <= 10;
            }).ToList();
        }

        public List<Entity> GetCloseByEntities(Creature creature)
        {
            return _entities.Where(x =>
            {
                var xDiff = x.Position.Item1 - creature.Position.Item1;
                var yDiff = x.Position.Item2 - creature.Position.Item2;

                return Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2)) <= creature.SightRange;
            }).ToList();
        }

        public Map WorldMap
        {
            get => _worldMap;
            set => _worldMap = value;
        }
    }
}