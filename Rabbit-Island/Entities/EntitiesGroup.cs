using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rabbit_Island.Entities
{
    /// <summary>
    /// Represents a group of entities.
    /// </summary>
    internal class EntitiesGroup : Entity
    {
        /// <summary>
        /// All entities in a group.
        /// </summary>
        public List<Entity> Entities { get; }

        public EntitiesGroup(List<Entity> entities) : base(0, 0)
        {
            Entities = entities;
        }

        public override void DrawSelf(Canvas canvas)
        {
            throw new NotImplementedException();
        }
    }
}