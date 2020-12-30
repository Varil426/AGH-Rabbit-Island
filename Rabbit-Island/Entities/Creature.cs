﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
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

            _energyDrain = 10;

            _timeOfLastAction = DateTime.Now;
        }

        protected DateTime _timeOfLastAction;

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
            Position = Position + direction * (float)distance;
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
            Mating
        }

        public enum GenderType
        {
            Male,
            Female
        }

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

        protected virtual void UpdateStateSelf()
        {
            // TODO Change this
            var timeRate = World.Instance.WorldConfig.TimeRate;
            var timeDiff = (DateTime.Now - _timeOfLastAction).TotalMinutes;

            Energy -= (float)(_energyDrain * timeDiff * timeRate);

            if (Energy <= 0)
            {
                States.Remove(State.Alive);
                States.Add(State.Dead);
            }
        }

        protected abstract Action Think(List<Entity> closeByEntities);

        protected abstract void PerformAction(Action action);

        private List<Entity> GetCloseByEntites()
        {
            return World.Instance.GetCloseByEntities(this);
        }

        protected enum ActionType
        {
            MoveTo,
            Eat,
            // TODO Add more actions
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