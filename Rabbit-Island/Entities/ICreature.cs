using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rabbit_Island.Entities
{
    internal interface ICreature
    {
        public void Act();

        public Thread? CreatureThread { get; set; }

        public double Health { get; }

        public double Energy { get; }

        public double SightRange { get; }

        public double InteractionRange { get; }

        public double MovementSpeed { get; }

        public Creature.GenderType Gender { get; }

        public HashSet<Creature.State> States { get; }

        public DateTime DeathAt { get; }

        public bool IsAlive { get; }

        public DateTime PregnantAt { get; }

        public Creature? PregnantWith { get; }

        public void LoseHealth(float damage);

        public bool CanMate { get; }
    }
}