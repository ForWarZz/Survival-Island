﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Survival_Island.Entites.Base
{
    public abstract class EntiteBase
    {
        public FrameworkElement CanvaElement { get; protected set; }

        protected Canvas Carte { get; }

        private bool estStatique;

        private double posX, posY;

        private Collision collision;

        public EntiteBase(Canvas carte, bool estStatique)
        {
            Carte = carte;
            this.estStatique = estStatique;
        }

        public double PositionX
        {
            get
            {
                if (estStatique)
                    return posX;
                else
                    return Canvas.GetLeft(CanvaElement);
            }
            set
            {
                if (estStatique)
                    posX = value;

                Canvas.SetLeft(CanvaElement, value);
            }
        }

        public double PositionY
        {
            get
            {
                if (estStatique)
                    return posY;
                else
                    return Canvas.GetTop(CanvaElement);
            }
            set
            {
                if (estStatique)
                    posY = value;

                Canvas.SetTop(CanvaElement, value);
            }
        }

        public Point Position
        {
            get
            {
                return new Point(PositionX, PositionY);
            }
        }

        public Collision CollisionRectangle
        {
            get
            {
                if (estStatique)
                    return collision;

                return new Collision(PositionX, PositionY, CanvaElement.Width, CanvaElement.Height, AngleRotation());
            }
        }

        public Point Centre
        {
            get
            {
                return new Point(PositionX + CanvaElement.Width / 2, PositionY + CanvaElement.Height / 2);
            }
        }

        public bool EnCollisionAvec(EntiteBase objetCollision)
        {
            return EnCollisionAvec(objetCollision.CollisionRectangle);
        }

        public bool EnCollisionAvec(Collision collision)
        {
            return CollisionRectangle.EnCollisionAvec(collision);
        }

        public bool EnCollisionAvec(Rect rect)
        {
            return EnCollisionAvec(rect, 0);
        }

        public bool EnCollisionAvec(Rect rect, double angle)
        {
            return CollisionRectangle.EnCollisionAvec(new Collision(rect, angle));
        }

        public virtual void Apparaitre(Point position)
        {
            if (Carte == null)
                throw new Exception("La carte n'est pas définie");

            PositionX = position.X;
            PositionY = position.Y;

            collision = new Collision(position, CanvaElement.Width, CanvaElement.Height, AngleRotation());

            Carte.Children.Add(CanvaElement);
            //Console.WriteLine("DEBUG: Apparition de l'entité: X=" + PositionX + " Y=" + PositionY);
        }

        public virtual void Disparaitre()
        {
            if (Carte == null)
                throw new Exception("La carte n'est pas définie");

            Carte.Children.Remove(CanvaElement);
            //Console.WriteLine("DEBUG: Suppression de l'entité");
        }

        public double AngleRotation()
        {
            double angle = 0;

            Transform transformer = CanvaElement.RenderTransform;
            if (transformer != null && transformer is RotateTransform)
                angle = ((RotateTransform)transformer).Angle;

            return angle;
        }
    }
}
