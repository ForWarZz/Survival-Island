using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Survival_Island.Entites.Base
{
    public abstract class EntiteBase
    {
        public FrameworkElement canvaElement { get; protected set; }

        protected Canvas carte;

        private bool estStatique;

        private double posX;
        private double posY;

        private Collision collision;

        public EntiteBase(Canvas carte, bool estStatique)
        {
            this.carte = carte;
            this.estStatique = estStatique;
        }

        public double PositionX
        {
            get
            {
                if (estStatique)
                    return posX;
                else
                    return Canvas.GetLeft(canvaElement);
            }
            set
            {
                if (estStatique)
                    posX = value;

                Canvas.SetLeft(canvaElement, value);
            }
        }

        public double PositionY
        {
            get
            {
                if (estStatique)
                    return posY;
                else
                    return Canvas.GetTop(canvaElement);
            }
            set
            {
                if (estStatique)
                    posY = value;

                Canvas.SetTop(canvaElement, value);
            }
        }

        public Collision CollisionRectangle
        {
            get
            {
                if (estStatique)
                    return collision;

                return new Collision(new Rect(PositionX, PositionY, canvaElement.Width, canvaElement.Height), AngleRotation());
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

        public virtual void Apparaitre(double x, double y)
        {
            if (carte == null)
                throw new Exception("La carte n'est pas définie");

            PositionX = x;
            PositionY = y;

            collision = new Collision(new Rect(x, y, canvaElement.Width, canvaElement.Height), AngleRotation());

            carte.Children.Add(canvaElement);
        }

        public virtual void Disparaitre()
        {
            if (carte == null)
                throw new Exception("La carte n'est pas définie");

            carte.Children.Remove(canvaElement);
        }

        public double AngleRotation()
        {
            double angle = 0;

            Transform transformer = canvaElement.RenderTransform;
            if (transformer != null && transformer is RotateTransform)
                angle = ((RotateTransform)transformer).Angle;

            return angle;
        }
    }
}
