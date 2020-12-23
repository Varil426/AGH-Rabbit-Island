using System;
using System.Collections.Generic;
using System.Text;

namespace Rabbit_Island.Entities
{
    internal abstract class Creature : Entity, ICreature
    {
        protected Creature(float x, float y) : base(x, y)
        {
            // TODO Dodać ustawianie parametrów
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

        public float Health { get; set; }

        public float Energy { get; set; }

        public float SightRange { get; }

        public float InteractionRange { get; }

        public GenderType Gender { get; }

        public HashSet<State> States { get; }

        public DateTime DeathAt { get; }

        public abstract void Act();
    }
}