using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CollisionDetection;

namespace Survival_Island.Outils.Entites
{
    public abstract class EntiteBase
    {
        public FrameworkElement canvaElement { get; protected set; }
        protected Canvas carte;
        private bool estStatique;
        private double posX;
        private double posY;
        private double angleRotation; // Ajouter une variable pour la rotation
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

        // Ajouter l'angle de rotation pour la collision
        public double Rotation
        {
            get { return angleRotation; }
            set { angleRotation = value; }
        }

        public Collision CollisionRectangle
        {
            get
            {
                if (estStatique)
                    return collision;

                // Utiliser le constructeur de Collision avec l'angle de rotation
                return new Collision(new Rect(PositionX, PositionY, canvaElement.Width, canvaElement.Height), angleRotation);
            }
        }

        public bool EnCollisionAvec(EntiteBase objetCollision)
        {
            return CollisionRectangle.IntersectsWith(objetCollision.CollisionRectangle);
        }

        public virtual void Apparaitre(double x, double y)
        {
            if (carte == null)
                throw new Exception("La carte n'est pas définie");

            PositionX = x;
            PositionY = y;

            // Créer une collision dynamique avec la rotation
            collision = new Collision(new Rect(x, y, canvaElement.Width, canvaElement.Height), angleRotation);

            carte.Children.Add(canvaElement);
        }

        public virtual void Disparaitre()
        {
            if (carte == null)
                throw new Exception("La carte n'est pas définie");

            carte.Children.Remove(canvaElement);
        }

        public void AfficherCollision()
        {
            // Afficher la collision sous forme de polygone
            PointCollection points = new PointCollection(CollisionRectangle.Points);
            Polygon polygon = new Polygon
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2,
                Fill = Brushes.Transparent,
                Points = points
            };

            carte.Children.Add(polygon);
        }
    }
}
