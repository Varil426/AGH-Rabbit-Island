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
    }
}