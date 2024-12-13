using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace Survival_Island.Outils.Entites
{
    public abstract class EntiteBase
    {
        public FrameworkElement canvaElement { get; protected set; }

        protected Canvas carte;

        private bool estStatique;
        private double posX;
        private double posY;
        private Rect rect;


        public EntiteBase(Canvas carte, bool estStatique)
        {
            this.carte = carte;
            this.estStatique = estStatique;
        }

        public double positionX
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

        public double positionY
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

        public Rect collisionRectangle
        {
            get
            {
                if (estStatique)
                    return rect;
                return new Rect(positionX, positionY, canvaElement.Width, canvaElement.Height);
            }

            set
            {
                rect = value;
                MatrixTransform transform = canvaElement.RenderTransform as MatrixTransform;

                if (transform != null)
                {
                    rect = transform.TransformBounds(rect);
                }
            }
        }

        public bool EnCollisionAvec(EntiteBase objetCollision)
        {
            return collisionRectangle.IntersectsWith(objetCollision.collisionRectangle);
        }

        public bool EnCollisionAvec(Rect rect)
        {
            return collisionRectangle.IntersectsWith(rect);
        }

        public virtual void Apparaitre(double x, double y)
        {
            if (carte == null)
                throw new Exception("La carte n'est pas définie");

            if (estStatique)
                collisionRectangle = new Rect(x, y, canvaElement.Width, canvaElement.Height);

            positionX = x;
            positionY = y;

            carte.Children.Add(canvaElement);
        }

        public virtual void Disparaitre()
        {
            if (carte == null)
                throw new Exception("La carte n'est pas définie");

            carte.Children.Remove(canvaElement);
        }
    }
}
