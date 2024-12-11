using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survival_Island.joueur
{
    internal class JoueurCaracteristique
    {
        public double vie { get; set; } = 100;
        public double vieMax { get; set; } = 100;

        public double vitesse { get; set; } = 1;
        public double degat { get; set; } = 10;

        public double tempsRechargementCanon { get; set; } = 0.1;
    }
}
