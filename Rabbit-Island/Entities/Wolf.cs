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
                int timeScalar = 1000 / (int)World.Instance.WorldConfig.TimeRate;
                EatingTime = 120 * timeScalar; //TODO może zwiększyć czas
                MatingTime = 300 * timeScalar;
                WaitToMateTime = 50 * timeScalar;
                PregnancyTime = 3600 * 24 * 3 * timeScalar;
                MoveInOneDirectionTime = 300 * timeScalar;
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
        }

        public Wolf(Vector2 position) : base(position)
        {
            MaxHealth = StaticRandom.Generator.Next(90, 110);
            Health = MaxHealth;
            MaxEnergy = StaticRandom.Generator.Next(90, 110);
            Energy = MaxEnergy;
            SightRange = StaticRandom.Generator.Next(50);
            MovementSpeed = StaticRandom.Generator.Next(5, 20);
            InteractionRange = StaticRandom.Generator.Next(10);
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
                        creature.LoseHealth(Attack);
                    }
                    else
                    {
                        throw new Exception("Invalid target");
                    }
                    break;

                case ActionType.Eat:
                    Thread.Sleep(RaceValues.EatingTime);
                    if (World.Instance.RemoveEntity(action.Target)) //TODO usuwanie wątku królika
                    {
                        var energy = Energy + 50;
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
            if (Energy < MaxEnergy / 2)
            {
                if (States.Add(State.SearchingForFood))
                {
                    _movingSince = DateTime.Now;
                }
                // sprawdza czy w zasięgu wzroku jest królik:
                if (closeByEntities.Find(entity => entity is Rabbit) is Rabbit rabbit)
                {
                    // sprawdza czy królik jest w zasięgu interakcji:
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
                    // jeśli nie jest wtedy wilk goni królika:
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
            // sprawdza czy w zasięgu wzroku jest wilk:
            else if (closeByEntities.Find(entity => entity is Wolf wolf
                && wolf.IsAlive
                && wolf.Gender != Gender
                && !wolf.IsPregnant
                && !IsPregnant) is Wolf anotherWolf)
            {
                // sprawdza czy anotherWolf jest innej płci
                if (this.Gender != anotherWolf.Gender)
                {
                    // sprawdza czy anotherWolf jest w zasięgu interakcji:
                    if (Vector2.Distance(this.Position, anotherWolf.Position) <= this.InteractionRange)
                    {
                        return new Action(ActionType.Mate, anotherWolf);
                    }
                    // jeśli nie jest wtedy wilk goni anotherWolf:
                    return new Action(ActionType.MoveTo, anotherWolf);
                }
            }

            return new Action(ActionType.Nothing, this);

            // var destination = new Vector2(400, 400);
            // return new Action(ActionType.MoveTo, new Point(destination));
        }

        protected void UpdatePregnancyStatus()
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

            UpdatePregnancyStatus();
        }
    }
}