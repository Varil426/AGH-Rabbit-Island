using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rabbit_Island.Entities
{
    internal class Rabbit : Creature
    {
        // TODO Move this and wolves race values to common class from which that class derives
        public static class RaceValues
        {
            /// <summary>
            /// Scales values to match the Time Rate of the World.
            /// </summary>
            public static void RefreshValues()
            {
                // Time in seconds scaled to simulation time rate
                var timeScalar = 1000 / World.Instance.WorldConfig.TimeRate;
                EatingTime = (int)(120 * timeScalar);
                MatingTime = (int)(300 * timeScalar);
                WaitToMateTime = (int)(50 * timeScalar);
                PregnancyTime = (int)(3600 * 24 * World.Instance.WorldConfig.RabbitConfig.PregnancyDuration * timeScalar);
                MoveInOneDirectionTime = (int)(300 * timeScalar);
                LifeExpectancy = (int)(3600 * 24 * 15 * timeScalar); // TODO Create config input for that value (now 15 days)
            }

            /// <summary>
            /// Time needed to eat a fruit. (Scaled to simulation time rate)
            /// </summary>
            public static int EatingTime { get; private set; }

            /// <summary>
            /// Time neede to mate. (Scaled to simulation time rate)
            /// </summary>
            public static int MatingTime { get; private set; }

            /// <summary>
            /// Wait for other rabbit to mate time. (Scaled to simulation time rate)
            /// </summary>
            public static int WaitToMateTime { get; private set; }

            /// <summary>
            /// Time that pregnancy takes. (Scaled to simulation time rate)
            /// </summary>
            public static int PregnancyTime { get; private set; }

            /// <summary>
            /// Represents how long rabbit should move in one direction while searching for food. (Scaled to simulation time rate)
            /// </summary>
            public static int MoveInOneDirectionTime { get; private set; }

            /// <summary>
            /// Represents how long should rabbit live until death from natural causes. (Scaled to simulation time rate)
            /// </summary>
            public static int LifeExpectancy { get; private set; }
        }

        public Rabbit(Vector2 position) : base(position)
        {
            MaxHealth = StaticRandom.Generator.Next(90, 110);
            Health = MaxHealth;
            MaxEnergy = StaticRandom.Generator.Next(90, 110);
            Energy = MaxEnergy;
            SightRange = StaticRandom.Generator.Next(25, 50);
            MovementSpeed = StaticRandom.Generator.Next(5, 20);
            InteractionRange = StaticRandom.Generator.Next(2, 6);
            Fear = StaticRandom.Generator.Next(10);
        }

        public Rabbit(
            Vector2 position, float maxHealth, float maxEnergy, float sightRange, double movementSpeed, float interactionRange,
            int fear) : base(position)
        {
            MaxHealth = maxHealth;
            Health = MaxHealth;
            MaxEnergy = maxEnergy;
            Energy = MaxEnergy;
            SightRange = sightRange;
            MovementSpeed = movementSpeed;
            InteractionRange = interactionRange;
            Fear = fear;
        }

        public int Fear { get; }

        public override void DrawSelf(Canvas canvas)
        {
            var rabbitCanvas = new Canvas();
            var color = IsAlive ? Brushes.Blue : Brushes.Purple;
            var rectangle = new Rectangle()
            {
                Width = 1,
                Height = 1,
                Fill = color
            };
            rabbitCanvas.Children.Add(rectangle);
            if (World.Instance.WorldConfig.DrawRanges && IsAlive)
            {
                var sightRange = new Ellipse()
                {
                    Width = SightRange,
                    Height = SightRange,
                    Stroke = color
                };
                rabbitCanvas.Children.Add(sightRange);
                Canvas.SetLeft(sightRange, -SightRange / 2);
                Canvas.SetTop(sightRange, -SightRange / 2);
            }
            canvas.Children.Add(rabbitCanvas);
            Canvas.SetLeft(rabbitCanvas, Position.X);
            Canvas.SetTop(rabbitCanvas, Position.Y);
        }

        protected override void PerformAction(Action action)
        {
            switch (action.Type)
            {
                case ActionType.MoveTo:
                    Move(action.Target);
                    break;

                case ActionType.MoveAway:
                    MoveAway(action.Target);
                    break;

                case ActionType.Eat:
                    Thread.Sleep(RaceValues.EatingTime);
                    if (World.Instance.RemoveEntity(action.Target))
                    {
                        var energy = Energy + 50;
                        if (energy > MaxEnergy)
                            energy = MaxEnergy;
                        Energy = energy;
                    }
                    break;

                case ActionType.Mate:
                    if (action.Target is Rabbit otherRabbit)
                    {
                        if (otherRabbit.WaitingToMate)
                        {
                            otherRabbit.InteractionEvent.Set();
                            Thread.Sleep(RaceValues.MatingTime);
                            if (Gender == GenderType.Female)
                            {
                                PregnantAt = DateTime.Now;
                                PregnantWith = otherRabbit;
                                States.Add(State.Pregnant);
                            }
                        }
                        else
                        {
                            States.Add(State.WaitingToMate);
                            InteractionEvent.WaitOne(RaceValues.WaitToMateTime);
                            States.Remove(State.WaitingToMate);

                            Thread.Sleep(RaceValues.MatingTime);

                            if (Gender == GenderType.Female)
                            {
                                PregnantAt = DateTime.Now;
                                PregnantWith = otherRabbit;
                                States.Add(State.Pregnant);
                            }
                        }
                    }
                    break;

                case ActionType.Nothing:
                    break;

                default:
                    throw new Exception("Illegal action");
            }
        }

        protected override Action Think(List<Entity> closeByEntities)
        {
            // TODO Improve this
            if (closeByEntities.OfType<Wolf>().Any())
            {
                return new Action(ActionType.MoveAway, closeByEntities.OfType<Wolf>().First());
            }
            if (Energy < MaxEnergy / 2)
            {
                if (States.Add(State.SearchingForFood))
                {
                    _movingSince = DateTime.Now;
                }
                if (closeByEntities.Find(entity => entity is Fruit) is Fruit fruit)
                {
                    if (Vector2.Distance(Position, fruit.Position) <= InteractionRange)
                    {
                        States.Remove(State.SearchingForFood);
                        return new Action(ActionType.Eat, fruit);
                    }
                    return new Action(ActionType.MoveTo, fruit);
                }
                else if (_movingSince.AddMilliseconds(RaceValues.MoveInOneDirectionTime) <= DateTime.Now)
                {
                    var possibleValues = Enum.GetValues(typeof(RelativePosition.Direction)).Length;
                    var direction = (RelativePosition.Direction)StaticRandom.Generator.Next(possibleValues);
                    _movingSince = DateTime.Now;
                    _moveDirection = new RelativePosition(this, direction);
                    return new Action(ActionType.MoveTo, _moveDirection);
                }
                else
                {
                    return new Action(ActionType.MoveTo, _moveDirection);
                }
            }
            else if (closeByEntities.Find(entity => entity is Rabbit rabbit
                && rabbit.IsAlive
                && rabbit.Gender != Gender
                && rabbit.CanMate
                && !rabbit.IsPregnant
                && !IsPregnant) is Rabbit otherRabbit)
            {
                States.Remove(State.SearchingForMatingPartner);
                if (Vector2.Distance(Position, otherRabbit.Position) <= InteractionRange)
                {
                    return new Action(ActionType.Mate, otherRabbit);
                }
                return new Action(ActionType.MoveTo, otherRabbit);
            }
            else if (!States.Contains(State.Pregnant))
            {
                if (States.Add(State.SearchingForMatingPartner))
                {
                    _movingSince = DateTime.Now;
                }
                if (_movingSince.AddMilliseconds(RaceValues.MoveInOneDirectionTime) <= DateTime.Now)
                {
                    var possibleValues = Enum.GetValues(typeof(RelativePosition.Direction)).Length;
                    var direction = (RelativePosition.Direction)StaticRandom.Generator.Next(possibleValues);
                    _movingSince = DateTime.Now;
                    _moveDirection = new RelativePosition(this, direction);
                    return new Action(ActionType.MoveTo, _moveDirection);
                }
                else
                {
                    return new Action(ActionType.MoveTo, _moveDirection);
                }
            }
            return new Action(ActionType.Nothing, this);
        }

        protected override void UpdatePregnancyStatus()
        {
            if (Gender == GenderType.Female
                && States.Contains(State.Pregnant)
                && PregnantAt.AddMilliseconds(RaceValues.PregnancyTime) <= DateTime.Now
                && PregnantWith != null)
            {
                var offspring = World.GenerateOffspring(this, PregnantWith);
                foreach (Creature creature in offspring)
                {
                    World.Instance.AddCreature(creature);
                }
                States.Remove(State.Pregnant);
            }
        }

        protected override void UpdateStateSelf()
        {
            // TODO Add rabbit specific states updates
            base.UpdateStateSelf();
            // Rabbit specific status updates
        }

        protected override void DeathFromOldAge()
        {
            if (World.Instance.WorldConfig.DeathFromOldAge && CreatedAt.AddMilliseconds(RaceValues.LifeExpectancy) <= DateTime.Now)
            {
                Die();
            }
        }
    }
}