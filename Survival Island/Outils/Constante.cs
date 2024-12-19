using System.Collections.Immutable;

namespace Survival_Island.Outils
{
    internal class Constante
    {
        public const double MUSIQUE_VOLUME = 0.2;
        public const int EXPERIENCE_MODE_TRICHE = 10000;


        public const int NOMBRE_CARREAUX_MER = 32;

        public const int NOMBRE_IMG_BATEAU_JOUEUR = 4;
        public const string EXT_IMG_BATEAU_JOUEUR = "png";

        public const int JOUEUR_VIE_MAX = 100;
        public const int AMELIO_VIE_MAX = 20;

        public const int JOUEUR_VITESSE = 300;  // Vitesse en px/s
        public const int AMELIO_VITESSE_MAX = 550;  // Vitesse en px/s
        public const double AMELIO_VITESSE = 40;  // Vitesse en px/s

        public const int JOUEUR_DEGATS = 40;
        public const int AMELIO_DEGATS = 5;

        public const double JOUEUR_RECHARGEMENT_CANON = 1.0;    // En seconde
        public static readonly TimeSpan TEMPS_REAPPARITION = TimeSpan.FromSeconds(5);

        public const double BATEAU_ACCELERATION = 200;  // Vitesse en px/s

        public const int JOUEUR_EXPERIENCE_MAX_N1 = 100;
        public const double MULTIPLICATEUR_NIVEAU = 1.5;

        public const int LARGEUR_NAVIRE = 50;
        public const int HAUTEUR_NAVIRE = 100;

        public const int VITESSE_BOULET = 500;  // Vitesse en px/s
        public const int TAILLE_BOULET_INIT = 10;

        public const int ILE_VIE_MAX = 1000;
        public const int AMELIO_VIE_ILE = 1000;
        public const int MARGE_ILE = 800;

        public const int NOMBRE_IMG_ROCHERS = 3;
        public const string EXT_IMG_ROCHERS = "png";
        public const int NOMBRE_ROCHERS_CARTE = 100;
        public const double MULTIPLICATEUR_TAILLE_ROCHER = 0.5;

        public const int VITESSE_ROTATION_BATEAU = 250;     // Vitesse en deg/s
        public const int TOLERANCE_ANGLE_ROTATION = 5;  // En degré

        public const int BASE_COFFRE_VIE = JOUEUR_DEGATS * 3;
        public const int BASE_COFFRE_EXPERIENCE = 60;
        public const int BASE_COFFRE_LARGEUR = 70;
        public const int BASE_COFFRE_HAUTEUR = 50;
        public const double BASE_COFFRE_PROBA_VIE = 0.5;

        public const int BORNE_MIN_APPARITION_COFFRE = 0;
        public const int BORNE_MAX_APPARITION_COFFRE = 5;
        public const int MAX_COFFRE_SIMULTANE = 10;
        public const double MULTIPLICATEUR_TAILLE_COFFRE = 0.5;

        public static readonly TimeSpan APPARITION_COFFRE_INTERVAL = TimeSpan.FromMinutes(0.2);

        public const int HAUTEUR_BARRE_VIE = 10;

        public const int RAYON_DETECTION_JOUEUR = 300;
        public const int MARGE_APPARITION_ENNEMI = 500;
        public const int TOLERANCE_CIBLE_ENNEMI = 100;   // En deg

        public const double DISTANCE_EVASION = 100.0;

        public const int TEMPS_ENTRE_VAGUE = 20;    // En seconde

        public const int VAGUE_MIN_ENNEMI = 1;
        public const int VAGUE_MAX_ENNEMI = 8;

        public const string MESSAGE_VAGUE_EN_COURS = "Les ennemis arrivent. Protégez l'île à tout prix !";
        public const string MESSAGE_VAGUE_EN_ATTENTE = "Allez cherchez des trésors pour vous restaurer. Les ennemis arrivent dans {0} secondes !";

        public const int VIE_BASE_ENNEMI = 100;
        public const int MULTIPLICATEUR_VIE_ENNEMI = 2;

        public const int DEGATS_BASE_ENNEMI = 10;
        public const int RECOMPENSE_EXP_ENNEMI_TUE = 25;

        public const double TEMPS_RECHARGEMENT_MIN_ENNEMI = 1.0;
        public const double TEMPS_RECHARGEMENT_BASE_ENNEMI = 1.0;
        public const double TEMPS_RECHARGEMENT_MULTIPLICATEUR_ENNEMI = 3.0;


        public static readonly double[] MODE_QUATRE = [4.0, 360.0, 1.1];
        public static readonly double[] MODE_DOUBLE = [2.0, 0.0, 1.1, 10.0];
        public static readonly double[] MODE_POMPE = [3.0, 20.0, 1.1];
        public static readonly double[] MODE_OCTOPUS = [8.0, 360.0, 1.3];
        public static readonly double[] MODE_QUATRO_PLUS = [4.0, 360.0, 0.8, 10.0, 30.0];
        public static readonly double[] MODE_TRIO = [3.0, 0.0, 1.0, 15.0, 15.0];
        public static readonly double[] MODE_MEGA = [1.0, 0.0, 1.8, 20.0, 60.0];
        public static readonly double[] MODE_EVENTAILLE = [6.0, 180.0, 1.0];
        public static readonly double[] MODE_SNIPER = [1.0, 0.0, 3.0, 40.0, 0.0];
        public static readonly double[] MODE_MK30 = [5.0, 35.0, 0.8, 20.0, 15.0];

        public const int MUSIQUE_DE_BASE = 0;
        public const int NB_MUSIQUE = 4;
        public const string MUSIQUE_EXT = "mp3";

        public const int SON_DE_BASE = 0;
        public const int NB_SON = 5;
        public const string SON_EXT = "wav";
    }
}
