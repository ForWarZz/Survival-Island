using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Survival_Island.carte
{
    internal class Collision
    {
        public FrameworkElement element { get; set; }
        protected Canvas carte { get; set; }

        public Collision(Canvas carte)
        {
            this.carte = carte;
        }

        public Collision()
        { }

        public bool EnCollisionAvec(Collision objetCollision)
        {
            return Rect().IntersectsWith(objetCollision.Rect());
        }

        public bool EnCollisionAvec(Rect rect)
        {
            return Rect().IntersectsWith(rect);
        }

        public Rect Rect()
        {
            return new Rect(Canvas.GetLeft(element), Canvas.GetTop(element), element.Width, element.Height);
        }

        public virtual void Apparaitre(double x, double y)
        {
            if (carte == null)
                throw new Exception("La carte n'est pas définie");

            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);

            carte.Children.Add(element);
        }

        public virtual void Apparaitre()
        {
            if (carte == null)
                throw new Exception("La carte n'est pas définie");

            carte.Children.Add(element);
        }

        public virtual void Disparaitre()
        {
            if (carte == null)
                throw new Exception("La carte n'est pas définie");

            carte.Children.Remove(element);
        }
    }
}
