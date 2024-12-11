using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survival_Island.joueur
{
    internal class NavireCaracteristique
    {
        public int vieMax { get; set; } = Constante.JOUEUR_VIE_MAX;

        public double vitesse { get; set; } = Constante.JOUEUR_VITESSE;
        public int degats { get; set; } = Constante.JOUEUR_DEGATS;

        public double tempsRechargementCanon { get; set; } = Constante.JOUEUR_RECHARGEMENT_CANON;
    }
}
