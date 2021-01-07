using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace Rabbit_Island.Entities
{
    internal abstract class Creature : Entity, ICreature
    {
        protected Creature(float x, float y) : base(x, y)
        {
            States = new HashSet<State>();
            States.Add(State.Alive);

            Random random = new Random();
            Array values = Enum.GetValues(typeof(GenderType));
            Gender = (GenderType)values.GetValue(random.Next(values.Length))!;

            InteractionEvent = new AutoResetEvent(false);

            _energyDrain = 2;

            _timeOfLastAction = DateTime.Now;
        }

        protected DateTime _timeOfLastAction;

        private Thread? _creatureThread;

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
            WaitingToMate
        }

        public enum GenderType
        {
            Male,
            Female
        }

        public bool WaitingToMate => States.Contains(State.WaitingToMate);

        public bool IsPregnant => States.Contains(State.Pregnant);

        /// <summary>
        /// Energy drain per real time minute
        /// </summary>
        protected readonly float _energyDrain;

        public float MaxHealth { get; protected set; }
        public float Health { get; set; }

        public float MaxEnergy { get; protected set; }
        public float Energy { get; set; }

        public float SightRange { get; protected set; }

        public float InteractionRange { get; protected set; }

        /// <summary>
        /// Creature movement speed in units per real time minute
        /// </summary>
        public double MovementSpeed { get; protected set; }

        public GenderType Gender { get; }

        public HashSet<State> States { get; }

        public DateTime DeathAt { get; protected set; }

        public DateTime PregnantAt { get; protected set; }

        public Creature? PregnantWith { get; protected set; }

        public bool IsAlive => States.Contains(State.Alive) ?
            States.Contains(State.Dead) ? throw new Exception("Creature should not be alive and dead at the same time") : true
            : false;

        protected virtual void UpdateStateSelf()
        {
            // TODO Change this
            var timeRate = World.Instance.WorldConfig.TimeRate;
            var timeDiff = (DateTime.Now - _timeOfLastAction).TotalMinutes;

            Energy -= (float)(_energyDrain * timeDiff * timeRate);

            if (Energy <= 0)
            {
                Die();
            }
        }

        protected abstract Action Think(List<Entity> closeByEntities);

        protected abstract void PerformAction(Action action);

        protected void Die()
        {
            if (States.Contains(State.Alive))
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

        protected enum ActionType
        {
            MoveTo,
            Eat,
            Mate,
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

        public void Act()
        {
            while (States.Contains(State.Alive))
            {
                var closeByEntites = GetCloseByEntites();
                UpdateStateSelf();
                var action = Think(closeByEntites);
                PerformAction(action);
                _timeOfLastAction = DateTime.Now;
                Thread.Sleep(20);
            }
        }
    }
}