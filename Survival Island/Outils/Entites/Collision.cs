using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;

namespace Survival_Island.Outils.Entites
{
    public class Collision
    {
        private Point p1, p2, p3, p4;

        public Collision(Point p1, Point p2, Point p3, Point p4)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
        }

        public Collision(Rect rect, double angle)
        {
            Point centre = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            RotateTransform transformer = new RotateTransform(angle, centre.X, centre.Y);

            p1 = transformer.Transform(new Point(rect.X, rect.Y));
            p2 = transformer.Transform(new Point(rect.X + rect.Width, rect.Y));
            p3 = transformer.Transform(new Point(rect.X + rect.Width, rect.Y + rect.Height));
            p4 = transformer.Transform(new Point(rect.X, rect.Y + rect.Height));
        }

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
            List<Point[]> segmentsA = new List<Point[]>
            {
                new Point[] { p1, p2 },
                new Point[] { p2, p3 },
                new Point[] { p3, p4 },
                new Point[] { p4, p1 }
            };

            List<Point[]> segmentsB = new List<Point[]>
            {
                new Point[] { autreRect.p1, autreRect.p2 },
                new Point[] { autreRect.p2, autreRect.p3 },
                new Point[] { autreRect.p3, autreRect.p4 },
                new Point[] { autreRect.p4, autreRect.p1 }
            };

            foreach (Point[] segmentA in segmentsA)
            {
                foreach (Point[] segmentB in segmentsB)
                {
                    if (LignesIntersectent(segmentA[0], segmentA[1], segmentB[0], segmentB[1]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
