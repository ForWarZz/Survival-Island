namespace Survival_Island.Outils
{
    internal class Constante
    {
        public const int NOMBRE_CARREAUX_MER = 32;

        public const int JOUEUR_VIE_MAX = 100;
        public const int AMELIO_VIE_MAX = 20;
        public const int JOUEUR_VITESSE = 4;
        public const int AMELIO_VITESSE_MAX = 8;
        public const double AMELIO_VITESSE = 0.2;
        public const int JOUEUR_DEGATS = 20;
        public const int AMELIO_DEGATS = 2;
        public const double JOUEUR_RECHARGEMENT_CANON = 1;

        public const double BATEAU_ACCELERATION = 0.1;

        public const int JOUEUR_EXPERIENCE_MAX_N1 = 100;
        public const double MULTIPLICATEUR_NIVEAU = 1.5;

        public const int LARGEUR_NAVIRE = 50;
        public const int HAUTEUR_NAVIRE = 100;

        public const int VITESSE_BOULET = 10;
        public const int TAILLE_BOULET_INIT = 10;

        public const int ILE_VIE_MAX = 1000;
        public const int AMELIO_VIE_ILE = 1000;

        public const int NOMBRE_ROCHERS_CARTE = 200;
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

        public static readonly TimeSpan APPARITION_COFFRE_INTERVAL = TimeSpan.FromMinutes(0.2);

        public const int HAUTEUR_BARRE_VIE = 10;

        public const int TAILLE_CELLULE_RECHERCHE_CHEMIN = 15;
    }
}
