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

        // Pour le débogage
        private Polygon? debugPolygon;

        public Collision(Rect rect, double angle = 0)
        {
            Point centre = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            RotateTransform transformer = new RotateTransform(angle, centre.X, centre.Y);

            corners =
            [
                transformer.Transform(new Point(rect.X, rect.Y)),
                transformer.Transform(new Point(rect.X + rect.Width, rect.Y)),
                transformer.Transform(new Point(rect.X + rect.Width, rect.Y + rect.Height)),
                transformer.Transform(new Point(rect.X, rect.Y + rect.Height))
            ];

            Segments =
            [
                [corners[0], corners[1]],
                [corners[1], corners[2]],
                [corners[2], corners[3]],
                [corners[3], corners[0]]
            ];
        }

        public Collision(Point position, double largeur, double hauteur, double angle = 0) : this(new Rect(position.X, position.Y, largeur, hauteur), angle)
        { }

        public Collision(double posX, double posY, double largeur, double hauteur, double angle = 0) : this(new Rect(posX, posY, largeur, hauteur), angle)
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

            foreach (Point point in autreRect.corners)
            {
                if (PointDansCollision(point))
                {
                    return true;
                }
            }

            foreach (Point point in corners)
            {
                if (autreRect.PointDansCollision(point))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CollisionDevantAvec(Collision autreRect)
        {
            Point[] segmentDevant = Segments[2];

            foreach (Point[] segmentB in autreRect.Segments)
            {
                if (LignesIntersectent(segmentDevant[0], segmentDevant[1], segmentB[0], segmentB[1]))
                {
                    return true;
                }
            }

            return false;
        }

        public bool PointDansCollision(Point point)
        {
            // Le point doit être à l'intérieur des bornes en X et Y
            double minX = Math.Min(corners[0].X, Math.Min(corners[1].X, Math.Min(corners[2].X, corners[3].X)));
            double maxX = Math.Max(corners[0].X, Math.Max(corners[1].X, Math.Max(corners[2].X, corners[3].X)));
            double minY = Math.Min(corners[0].Y, Math.Min(corners[1].Y, Math.Min(corners[2].Y, corners[3].Y)));
            double maxY = Math.Max(corners[0].Y, Math.Max(corners[1].Y, Math.Max(corners[2].Y, corners[3].Y)));

            return point.X >= minX && point.X <= maxX &&
                   point.Y >= minY && point.Y <= maxY;
        }

        // Fonction de debogugage permettant de visualier la collision box
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
