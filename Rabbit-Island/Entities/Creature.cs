using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace Rabbit_Island.Entities
{
    internal abstract class Creature : Entity, ICreature
    {
        protected Creature(Vector2 position, uint generation) : this(position)
        {
            Generation = generation;
        }

        protected Creature(Vector2 position) : base(position)
        {
            States = new HashSet<State>
            {
                State.Alive
            };

            Array values = Enum.GetValues(typeof(GenderType));
            Gender = (GenderType)values.GetValue(StaticRandom.Generator.Next(values.Length))!;

            InteractionEvent = new AutoResetEvent(false);

            _energyDrain = 0.05f;

            _timeOfLastAction = DateTime.Now;
            _movingSince = DateTime.Now;
            _moveDirection = new RelativePosition(this, RelativePosition.Direction.North);
        }

        protected DateTime _timeOfLastAction;

        protected Thread? _creatureThread;

        protected bool _threadRun;

        protected DateTime _movingSince;

        protected RelativePosition _moveDirection;

        public Thread? CreatureThread
        {
            get => _creatureThread;
            set
            {
                if (_creatureThread == null)
                    _creatureThread = value;
            }
        }

        protected AutoResetEvent InteractionEvent { get; }

        protected void MoveAway(EntitiesGroup entitiesGroup)
        {
            var now = DateTime.Now;
            var timeDifference = now - _timeOfLastAction;

            var directions = new List<Vector2>();
            foreach (var entity in entitiesGroup.Entities)
            {
                directions.Add(Vector2.Normalize(Position - entity.Position));
            }

            Vector2 finalDirection = new Vector2();
            foreach (var direction in directions)
            {
                finalDirection += direction;
            }
            finalDirection = Vector2.Normalize(finalDirection);

            var distance = MovementSpeed * timeDifference.TotalMinutes * World.Instance.WorldConfig.TimeRate;
            var newPosition = Position + finalDirection * (float)distance;
            if (newPosition.X <= World.Instance.WorldMap.Size.Item1
                && newPosition.X >= 0
                && newPosition.Y <= World.Instance.WorldMap.Size.Item2
                && newPosition.Y >= 0)
            {
                Position = newPosition;
            }
        }

        protected void MoveAway(Entity entityToMoveAwayFrom)
        {
            var now = DateTime.Now;
            var timeDifference = now - _timeOfLastAction;

            var direction = Vector2.Normalize(Position - entityToMoveAwayFrom.Position);

            var distance = MovementSpeed * timeDifference.TotalMinutes * World.Instance.WorldConfig.TimeRate;
            var newPosition = Position + direction * (float)distance;
            if (newPosition.X <= World.Instance.WorldMap.Size.Item1
                && newPosition.X >= 0
                && newPosition.Y <= World.Instance.WorldMap.Size.Item2
                && newPosition.Y >= 0)
            {
                Position = newPosition;
            }
        }

        protected void Move(Entity destination)
        {
            Move(destination.Position);
        }

        protected void Move(Vector2 destination)
        {
            var now = DateTime.Now;
            var timeDifference = now - _timeOfLastAction;

            var direction = Vector2.Normalize(destination - Position);

            var distance = MovementSpeed * timeDifference.TotalMinutes * World.Instance.WorldConfig.TimeRate;
            var newPosition = Position + direction * (float)distance;
            if (newPosition.X <= World.Instance.WorldMap.Size.Item1
                && newPosition.X >= 0
                && newPosition.Y <= World.Instance.WorldMap.Size.Item2
                && newPosition.Y >= 0)
            {
                Position = newPosition;
            }
        }

        public enum State
        {
            Thinking,
            Moving,
            Eating,
            Hungry,
            Pregnant,
            Alive,
            Dead,
            SearchingForFood,
            Mating,
            WaitingToMate,
            SearchingForMatingPartner
        }

        public enum GenderType
        {
            Male,
            Female
        }

        public bool WaitingToMate => States.Contains(State.WaitingToMate);

        public bool IsPregnant => States.Contains(State.Pregnant);

        /// <summary>
        /// Energy drain per minute.
        /// </summary>
        protected readonly double _energyDrain;

        public double MaxHealth { get; protected set; }
        public double Health { get; protected set; }

        public double MaxEnergy { get; protected set; }
        public double Energy { get; protected set; }

        public double SightRange { get; protected set; }

        public double InteractionRange { get; protected set; }

        /// <summary>
        /// Creature movement speed in units per real time minute
        /// </summary>
        public double MovementSpeed { get; protected set; }

        public GenderType Gender { get; }

        public HashSet<State> States { get; }

        public DateTime DeathAt { get; protected set; }

        public DateTime PregnantAt { get; protected set; }

        public Creature? PregnantWith { get; protected set; }

        public uint Generation { get; }

        public bool CanMate => CreatedAt.AddMilliseconds((1000 * 60 * 60) / World.Instance.WorldConfig.TimeRate) <= DateTime.Now;

        protected abstract void UpdatePregnancyStatus();

        protected abstract void DeathFromOldAge();

        public bool IsAlive => States.Contains(State.Alive)
            && (States.Contains(State.Dead) ? throw new Exception("Creature should not be alive and dead at the same time") : true);

        protected virtual void UpdateStateSelf()
        {
            UpdatePregnancyStatus();
            DeathFromOldAge();
            // TODO Change this
            var timeRate = World.Instance.WorldConfig.TimeRate;
            var timeDiff = (DateTime.Now - _timeOfLastAction).TotalMinutes * timeRate;

            Energy -= (float)(_energyDrain * timeDiff);

            if (Energy <= 0 || Health <= 0)
            {
                Die();
            }
        }

        protected abstract Action Think(List<Entity> closeByEntities);

        protected abstract void PerformAction(Action action);

        protected void Die()
        {
            if (IsAlive)
            {
                States.Remove(State.Alive);
                States.Add(State.Dead);
                DeathAt = DateTime.Now;
            }
            else
            {
                throw new Exception("Die() was called on not alive creature");
            }
        }

        private List<Entity> GetCloseByEntites()
        {
            return World.Instance.GetCloseByEntities(this);
        }

        protected int AdditionalCost(double previousValue, double newValue)
        {
            var previousConst = previousValue / 2;
            var newCost = newValue / 2;

            return (int)(newCost - previousConst);
        }

        protected IEnumerable<TKey> RandomKeyFormDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            List<TKey> keys = Enumerable.ToList(dictionary.Keys);
            int size = dictionary.Count;
            while (true)
            {
                yield return keys[StaticRandom.Generator.Next(size)];
            }
        }

        protected enum ActionType
        {
            MoveTo,
            MoveAway,
            Eat,
            Mate,
            Attack,
            Nothing
        }

        protected class Action
        {
            public Action(ActionType type, Entity target)
            {
                Type = type;
                Target = target;
            }

            public ActionType Type { get; }

            public Entity Target { get; }
        }

        public void StopThread()
        {
            _threadRun = false;
            _creatureThread?.Interrupt();
        }

        public void Act()
        {
            _threadRun = true;
            while (IsAlive && _threadRun)
            {
                try
                {
                    var closeByEntites = GetCloseByEntites();
                    UpdateStateSelf();
                    var action = Think(closeByEntites);
                    PerformAction(action);
                    _timeOfLastAction = DateTime.Now;
                    Thread.Sleep(20);
                }
                catch (ThreadInterruptedException)
                {
                }
            }
        }

        public void LoseHealth(double damage)
        {
            lock (this)
            {
                Health -= damage;
            }
        }

        public class CreatureMap<T> : ClassMap<T> where T : Creature
        {
            public CreatureMap()
            {
                Map(creature => creature.CreatedAt).Name("createdAt");
                Map(creature => creature.DeathAt).Name("deathAt");
                Map(creature => creature.Generation).Name("generation");
                Map(creature => creature.MaxHealth).Name("maxHealth");
                Map(creature => creature.MaxEnergy).Name("maxEnergy");
                Map(creature => creature.MovementSpeed).Name("movementSpeed");
                Map(creature => creature.SightRange).Name("sightRange");
                Map(creature => creature.InteractionRange).Name("interactionRange");
                Map(creature => creature.Gender).Name("gender");
            }
        }
    }
}