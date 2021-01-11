using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;

namespace Rabbit_Island.Entities
{
    internal class Wolf : Creature
    {
        public static class RaceValues
        {
            /// <summary>
            /// Scales values to match the Time Rate of the World.
            /// </summary>
            public static void RefreshValues()
            {
                // Time in seconds scaled to simulation time rate
                var timeScalar = 1000 / World.Instance.WorldConfig.TimeRate;
                EatingTime = (int)(180 * timeScalar);
                MatingTime = (int)(300 * timeScalar);
                WaitToMateTime = (int)(50 * timeScalar);
                PregnancyTime = (int)(3600 * 24 * World.Instance.WorldConfig.WolvesConfig.PregnancyDuration * timeScalar);
                MoveInOneDirectionTime = (int)(300 * timeScalar);
                LifeExpectancy = (int)(3600 * 24 * World.Instance.WorldConfig.WolvesConfig.LifeExpectancy * timeScalar);
            }

            /// <summary>
            /// Time needed to eat a rabbit. (Scaled to simulation time rate)
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
            /// Represents how long wolf should move in one direction while searching for rabbit. (Scaled to simulation time rate)
            /// </summary>
            public static int MoveInOneDirectionTime { get; private set; }

            /// <summary>
            /// Represents how long should wolf live until death from natural causes. (Scaled to simulation time rate)
            /// </summary>
            public static int LifeExpectancy { get; private set; }
        }

        public Wolf(Vector2 position) : base(position)
        {
            MaxHealth = StaticRandom.Generator.Next(90, 110);
            Health = MaxHealth;
            MaxEnergy = StaticRandom.Generator.Next(90, 110);
            Energy = MaxEnergy;
            SightRange = StaticRandom.Generator.Next(25, 50);
            MovementSpeed = StaticRandom.Generator.Next(5, 20);
            InteractionRange = StaticRandom.Generator.Next(4, 8);
            Attack = StaticRandom.Generator.Next(30, 120);
        }

        public Wolf(Vector2 position, float maxHealth, float maxEnergy, float sightRange, double movementSpeed, float interactionRange, int attack) : base(position)
        {
            MaxHealth = maxHealth;
            Health = MaxHealth;
            MaxEnergy = maxEnergy;
            Energy = MaxEnergy;
            SightRange = sightRange;
            MovementSpeed = movementSpeed;
            InteractionRange = interactionRange;
            Attack = attack;
        }

        public int Attack { get; }

        public override void DrawSelf(Canvas canvas)
        {
            var wolfCanvas = new Canvas();
            var color = IsAlive ? Brushes.Red : Brushes.Purple;
            var rectangle = new Rectangle()
            {
                Width = 1,
                Height = 1,
                Fill = color
            };
            wolfCanvas.Children.Add(rectangle);
            if (World.Instance.WorldConfig.DrawRanges && IsAlive)
            {
                var sightRange = new Ellipse()
                {
                    Width = SightRange,
                    Height = SightRange,
                    Stroke = color
                };
                wolfCanvas.Children.Add(sightRange);
                Canvas.SetLeft(sightRange, -SightRange / 2);
                Canvas.SetTop(sightRange, -SightRange / 2);
            }
            canvas.Children.Add(wolfCanvas);
            Canvas.SetLeft(wolfCanvas, Position.X);
            Canvas.SetTop(wolfCanvas, Position.Y);
        }

        protected override void PerformAction(Action action)
        {
            switch (action.Type)
            {
                case ActionType.MoveTo:
                    Move(action.Target);
                    break;

                case ActionType.Attack:
                    if (action.Target is Creature creature)
                    {
                        creature.CreatureThread?.Interrupt();
                        creature.LoseHealth(Attack);
                    }
                    else
                    {
                        throw new Exception("Invalid target");
                    }
                    break;

                case ActionType.Eat:
                    Thread.Sleep(RaceValues.EatingTime);
                    if (action.Target is Rabbit rabbit && !rabbit.IsAlive && World.Instance.RemoveEntity(action.Target))
                    {
                        var energy = Energy + 80;
                        if (energy > MaxEnergy)
                            energy = MaxEnergy;
                        Energy = energy;
                    }
                    break;

                case ActionType.Mate:
                    if (action.Target is Wolf anotherWolf)
                    {
                        if (anotherWolf.WaitingToMate)
                        {
                            anotherWolf.InteractionEvent.Set();
                            Thread.Sleep(RaceValues.MatingTime);
                            if (Gender == GenderType.Female)
                            {
                                PregnantAt = DateTime.Now;
                                PregnantWith = anotherWolf;
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
                                PregnantWith = anotherWolf;
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
            if (Energy < MaxEnergy / 3)
            {
                if (States.Add(State.SearchingForFood))
                {
                    _movingSince = DateTime.Now;
                }
                if (closeByEntities.Find(entity => entity is Rabbit) is Rabbit rabbit)
                {
                    if (Vector2.Distance(this.Position, rabbit.Position) <= InteractionRange)
                    {
                        if (rabbit.IsAlive)
                        {
                            return new Action(ActionType.Attack, rabbit);
                        }
                        else
                        {
                            States.Remove(State.SearchingForFood);
                            return new Action(ActionType.Eat, rabbit);
                        }
                    }
                    return new Action(ActionType.MoveTo, rabbit);
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
            else if (closeByEntities.Find(entity => entity is Wolf wolf
                && wolf.IsAlive
                && wolf.CanMate
                && wolf.Gender != Gender
                && !wolf.IsPregnant
                && !IsPregnant) is Wolf anotherWolf)
            {
                States.Remove(State.SearchingForMatingPartner);
                if (this.Gender != anotherWolf.Gender)
                {
                    if (Vector2.Distance(this.Position, anotherWolf.Position) <= this.InteractionRange)
                    {
                        return new Action(ActionType.Mate, anotherWolf);
                    }
                    return new Action(ActionType.MoveTo, anotherWolf);
                }
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
            // TODO Add wolf specific states updates
            base.UpdateStateSelf();
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