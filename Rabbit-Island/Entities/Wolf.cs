using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rabbit_Island.Entities
{
    internal class Wolf : Creature
    {
        public Wolf(Vector2 position) : base(position)
        {
            MaxHealth = StaticRandom.Generator.Next(90, 110);
            Health = MaxHealth;
            MaxEnergy = StaticRandom.Generator.Next(90, 110);
            Energy = MaxEnergy;
            SightRange = StaticRandom.Generator.Next(50);
            MovementSpeed = StaticRandom.Generator.Next(5, 20);
            InteractionRange = StaticRandom.Generator.Next(10);
        }

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

                case ActionType.Eat:
                    /* 
                    - zatrzymuje się na określony czas - sleep np. 30 sekund
                    - usuwany jest obiekt Królika (powinien mieć takie same współrzędne) 
                    - Energia jest dodawana
                    - ? Health jest dodawany ?
                    break;
                     */
                    throw new NotImplementedException();
                case ActionType.Mate:
                    /* 
                    //TODO
                    break;
                    */
                    throw new NotImplementedException();
                case ActionType.Nothing:
                    /* 
                    - tu chyba nic nie będzie
                    */
                    break;
                default:
                    throw new Exception("Illegal action");
            }
        }

        protected override Action Think(List<Entity> closeByEntities)
        {
            // TODO Improve this
            // sprawdza czy w zasięgu wzroku jest królik:
            if (closeByEntities.Find(entity => entity is Rabbit) is Rabbit rabbit)
            { 
                // sprawdza czy królik jest w zasięgu interakcji:
                if (Vector2.Distance(this.Position, rabbit.Position) <= this.InteractionRange)
                {
                    // jeśli jest zmienia akcję na Eat:
                    return new Action(ActionType.Eat, rabbit);
                }
                // jeśli nie jest wtedy wilk goni królika:
                return new Action(ActionType.MoveTo, rabbit);
            }

            // sprawdza czy w zasięgu wzroku jest wilk:
            if (closeByEntities.Find(entity => entity is Wolf) is Wolf anotherWolf)
            {
                // sprawdza czy anotherWolf jest innej płci
                if (this.Gender != anotherWolf.Gender)
                {
                    // sprawdza czy anotherWolf jest w zasięgu interakcji:
                    if (Vector2.Distance(this.Position, anotherWolf.Position) <= this.InteractionRange)
                    {
                        //TODO tu coś więcej warunków żeby zmienić stan na Mating
                        // - sprawdzić czy anotherWolf nie jest w ciąży / już mating
                    }
                    // jeśli nie jest wtedy wilk goni anotherWolf:
                    return new Action(ActionType.MoveTo, anotherWolf);
                } 
            }

            // przypadek ostatni (gdy nic się nie dopasowało) - idzie w dowolnym kierunku, wartości raczej powinny być losowe
            var destination = new Vector2(400, 400);
            return new Action(ActionType.MoveTo, new Point(destination));
        }

        protected override void UpdateStateSelf()
        {
            // TODO Add wolf specific states updates
            base.UpdateStateSelf();
            /*
             - tu jest tracona energia (napisane w klasie bazowej)
             - TODO co jeszcze?
             */
        }
    }
}