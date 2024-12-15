using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Survival_Island.Entites.Base
{
    public class Collision
    {
        public Point[][] Segments { get; }

        private Point[] corners;
        private Polygon? debugPolygon;

        public Collision(Rect rect, double angle)
        {
            Point centre = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            RotateTransform transformer = new RotateTransform(angle, centre.X, centre.Y);

            corners = [
                transformer.Transform(new Point(rect.X, rect.Y)),
                transformer.Transform(new Point(rect.X + rect.Width, rect.Y)),
                transformer.Transform(new Point(rect.X + rect.Width, rect.Y + rect.Height)),
                transformer.Transform(new Point(rect.X, rect.Y + rect.Height))
            ];

            Segments = [
                [ corners[0], corners[1] ],
                [ corners[1], corners[2] ],
                [ corners[2], corners[3] ],
                [ corners[3], corners[0] ]
            ];
        }

        public Collision(double posX, double posY, double largeur, double hauteur, double angle) : this(new Rect(posX, posY, largeur, hauteur), angle)
        { }

        public Collision(double posX, double posY, double largeur, double hauteur) : this(new Rect(posX, posY, largeur, hauteur), 0)
        { }

        private bool LignesIntersectent(Point A1, Point A2, Point B1, Point B2)
        {
            Vector v1 = A2 - A1;
            Vector v2 = B2 - B1;

            Vector v3 = B1 - A1;
            Vector v4 = A1 - B1;

            double denom = Vector.CrossProduct(v1, v2);
            if (denom == 0) return false;

            double t1 = Vector.CrossProduct(v3, v2) / denom;
            double t2 = Vector.CrossProduct(v3, v1) / denom;

            return t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1;
        }

        public bool EnCollisionAvec(Collision autreRect)
        {
            foreach (Point[] segmentA in Segments)
            {
                foreach (Point[] segmentB in autreRect.Segments)
                {
                    if (LignesIntersectent(segmentA[0], segmentA[1], segmentB[0], segmentB[1]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void AfficherCollision(Canvas carte)
        {
            Polygon debugRectangle = new Polygon();
            debugRectangle.Points = new PointCollection(corners);
            debugRectangle.Stroke = Brushes.Red;
            debugRectangle.StrokeThickness = 1;

            carte.Children.Add(debugRectangle);
        }
    }
}
