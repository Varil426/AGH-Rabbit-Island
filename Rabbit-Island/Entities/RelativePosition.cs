using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit_Island.Entities
{
    internal class RelativePosition : Point
    {
        public enum Direction
        {
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest
        }

        private const int DISTANCE_FROM_ENTITY = 100;

        private readonly Entity targetEntity;

        private readonly Direction targetDirection;

        /// <summary>
        /// Represents target that is in relative (to entity) position that matches direction (for that entity).
        /// </summary>
        /// <param name="entity">Entity that position is relative to.</param>
        /// <param name="direction">Direction.</param>
        public RelativePosition(Entity entity, Direction direction) : base(entity.Position.X, entity.Position.Y)
        {
            targetEntity = entity;
            targetDirection = direction;
        }

        public Entity TargetEntity => targetEntity;

        /// <summary>
        /// Returns some point in position relative to TargetEntity.
        /// </summary>
        public override Vector2 Position
        {
            get
            {
                var relativePosition = new Vector2();
                var targetX = targetEntity.Position.X;
                var targetY = targetEntity.Position.Y;
                switch (targetDirection)
                {
                    case Direction.North:
                        relativePosition.X = targetX;
                        relativePosition.Y = targetY - DISTANCE_FROM_ENTITY;
                        break;

                    case Direction.NorthEast:
                        relativePosition.X = targetX + DISTANCE_FROM_ENTITY;
                        relativePosition.Y = targetY - DISTANCE_FROM_ENTITY;
                        break;

                    case Direction.East:
                        relativePosition.X = targetX + DISTANCE_FROM_ENTITY;
                        relativePosition.Y = targetY;
                        break;

                    case Direction.SouthEast:
                        relativePosition.X = targetX + DISTANCE_FROM_ENTITY;
                        relativePosition.Y = targetY + DISTANCE_FROM_ENTITY;
                        break;

                    case Direction.South:
                        relativePosition.X = targetX;
                        relativePosition.Y = targetY + DISTANCE_FROM_ENTITY;
                        break;

                    case Direction.SouthWest:
                        relativePosition.X = targetX - DISTANCE_FROM_ENTITY;
                        relativePosition.Y = targetY + DISTANCE_FROM_ENTITY;
                        break;

                    case Direction.West:
                        relativePosition.X = targetX - DISTANCE_FROM_ENTITY;
                        relativePosition.Y = targetY;
                        break;

                    case Direction.NorthWest:
                        relativePosition.X = targetX - DISTANCE_FROM_ENTITY;
                        relativePosition.Y = targetY - DISTANCE_FROM_ENTITY;
                        break;
                }
                return relativePosition;
            }
            protected set
            {
            }
        }
    }
}