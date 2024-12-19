using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Survival_Island
{
    public class Calcul
    {
        public static double DistanceAvec(Point posA, Point posB)
        {
            return Math.Sqrt(Math.Pow(posA.X - posB.X, 2) + Math.Pow(posA.Y - posB.Y, 2));
        }
    }
}
