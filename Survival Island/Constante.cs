using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survival_Island
{
    internal class Constante
    {
        public const int NOMBRE_CARREAUX_MER = 40;

        public const int JOUEUR_VIE_MAX = 100;
        public const int JOUEUR_VITESSE = 5;
        public const int JOUEUR_DEGATS = 20;
        public const double JOUEUR_RECHARGEMENT_CANON = 1;

        public const int LARGEUR_NAVIRE = 50;
        public const int HAUTEUR_NAVIRE = 100;

        public const int VITESSE_BOULET = 10;
        public const int TAILLE_BOULET = 10;

        public const int ILE_VIE_MAX = 1000;

        public const int NOMBRE_CAILLOUX_CARTE = 20;

        public const int TEMPS_ROTATION_NAVIRE = 20;    // Millisecondes
        public const int TOLERANCE_ANGLE_ROTATION = 5;  // En degré

        public const int BASE_COFFRE_VIE = JOUEUR_DEGATS * 3;
        public const int BASE_COFFRE_EXPERIENCE = 50;
    }
}
