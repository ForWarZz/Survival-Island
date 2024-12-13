using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survival_Island.Outils
{
    internal class Constante
    {
        public const int NOMBRE_CARREAUX_MER = 40;

        public const int JOUEUR_VIE_MAX = 100;
        public const int JOUEUR_VITESSE = 5;
        public const int JOUEUR_DEGATS = 20;
        public const double JOUEUR_RECHARGEMENT_CANON = 1;

        public const int JOUEUR_EXPERIENCE_MAX_N1 = 100;
        public const double MULTIPLICATEUR_NIVEAU = 1.5;

        public const int LARGEUR_NAVIRE = 50;
        public const int HAUTEUR_NAVIRE = 100;

        public const int VITESSE_BOULET = 10;
        public const int TAILLE_BOULET = 10;

        public const int ILE_VIE_MAX = 1000;

        public const int NOMBRE_ROCHERS_CARTE = 20;
        public const double MULTIPILICATEUR_TAILLE_ROCHER = 0.5;

        public const int TEMPS_ROTATION_NAVIRE = 20;    // Millisecondes
        public const int TOLERANCE_ANGLE_ROTATION = 5;  // En degré

        public const int BASE_COFFRE_VIE = JOUEUR_DEGATS * 3;
        public const int BASE_COFFRE_EXPERIENCE = 30;
        public const int BASE_COFFRE_LARGEUR = 70;
        public const int BASE_COFFRE_HAUTEUR = 50;

        public const int BORNE_MIN_APPARITION_COFFRE = 0;
        public const int BORNE_MAX_APPARITION_COFFRE = 5;
        public const int MAX_COFFRE_SIMULTANE = 10;
        public const double MULTIPLICATEUR_TAILLE_COFFRE = 0.5;

        public static readonly TimeSpan APPARITION_COFFRE_INTERVAL = TimeSpan.FromMinutes(2);

        public const int HAUTEUR_BARRE_VIE = 10;


        public static readonly TimeSpan VITESSE_ACCELERATION = TimeSpan.FromMilliseconds(16);

        public const double  VITESSE_MAX = 5;
    }
}
