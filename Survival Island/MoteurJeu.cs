using Survival_Island.Entites;
using Survival_Island.Entites.Base;
using Survival_Island.Entites.Navire;
using Survival_Island.Entites.Objets;
using Survival_Island.Outils;
using Survival_Island.Recherche;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Survival_Island
{
    public class MoteurJeu
    {
        public MainWindow Fenetre { get; }

        private Canvas carte;
        private Image[] mer;

        private BitmapImage bitmapMer, bitmapTresor, bitmapBateau;
        private BitmapImage[] bitmapRochers;

        private DispatcherTimer objetBonusMinuteur;

        public Joueur Joueur { get; private set; }
        private Random random;

        public List<Boulet> Boulets { get; }
        public Obstacle[] Obstacles { get; }
        public List<ObjetRecompense> ObjetsBonus { get; }

        public Ile Ile { get; private set; }

        public RechercheChemin RechercheChemin { get; private set; }
        public List<Ennemi> Ennemis { get; }

        public MoteurJeu(MainWindow fenetre)
        {
            Fenetre = fenetre;

            carte = fenetre.carte;
            random = new Random();

            Boulets = new List<Boulet>();
            ObjetsBonus = new List<ObjetRecompense>();
            Obstacles = new Obstacle[Constante.NOMBRE_ROCHERS_CARTE];
            Ennemis = new List<Ennemi>();

            InitBitmaps();
            InitCarte();
        }

        public void InitJeu()
        {
            InitIle();
            InitRochers();

            InitRechercheChemin();

            InitJoueur();

            Fenetre.hudJoueur.Visibility = Visibility.Visible;
            Fenetre.btnAmeliorations.Visibility = Visibility.Visible;

            InitBonusMinuteur();
            InitBoucleJeu();
        }

        private void InitBitmaps()
        {
            bitmapMer = new BitmapImage(new Uri(Chemin.IMAGE_MER));
            bitmapRochers = [
                new BitmapImage(new Uri(Chemin.IMAGE_ROCHER1)),
                new BitmapImage(new Uri(Chemin.IMAGE_ROCHER2)),
                new BitmapImage(new Uri(Chemin.IMAGE_ROCHER3)),
            ];

            bitmapTresor = new BitmapImage(new Uri(Chemin.IMAGE_TRESOR));
            bitmapBateau = new BitmapImage(new Uri(Chemin.IMAGE_BATEAU_ROUGE));
        }

        private void InitIle()
        {
            Ile = new Ile(carte, Fenetre, this);
            Ile.Apparaitre();
        }

        private void InitJoueur()
        {
            double posX, posY;
            PositionAleatoireValide(Constante.LARGEUR_NAVIRE, Constante.HAUTEUR_NAVIRE, 0, out posX, out posY);

            Joueur = new Joueur(carte, this, bitmapBateau);
            Joueur.Apparaitre(100, 100);
        }

        private void InitRochers()
        {
            for (int i = 0; i < Obstacles.Length; i++)
            {
                Image rocher = new Image();
                BitmapImage randomRocher = bitmapRochers[random.Next(0, bitmapRochers.Length)];

                double angleRotation = random.Next(0, 361);
                double multiplicateurTaille = 0.5 + random.NextDouble();

                rocher.Source = randomRocher;
                rocher.Width = randomRocher.Width * multiplicateurTaille;
                rocher.Height = randomRocher.Height * multiplicateurTaille;
                rocher.RenderTransform = new RotateTransform(angleRotation, rocher.Width / 2, rocher.Height / 2);

                double posX, posY;
                PositionAleatoireValide(rocher.Width, rocher.Height, angleRotation, out posX, out posY);

                Obstacle obstacle = new Obstacle(carte, rocher);
                obstacle.Apparaitre(posX, posY);

                Obstacles[i] = obstacle;
            }
        }

        private void InitCarte()
        {
            mer = new Image[Constante.NOMBRE_CARREAUX_MER];
            carte.Width = mer.Length * bitmapMer.Width;
            carte.Height = mer.Length * bitmapMer.Height;

            // Initialisation de la mer en fond
            for (int i = 0; i < mer.Length; i++)
            {
                for (int j = 0; j < mer.Length; j++)
                {
                    mer[i] = new Image();
                    mer[i].Source = bitmapMer;
                    mer[i].Width = bitmapMer.Width;
                    mer[i].Height = bitmapMer.Height;

                    // Retire l'effet quadrilage des bords
                    RenderOptions.SetEdgeMode(mer[i], EdgeMode.Aliased);

                    Canvas.SetLeft(mer[i], j * bitmapMer.Width);
                    Canvas.SetTop(mer[i], i * bitmapMer.Height);

                    carte.Children.Add(mer[i]);
                }
            }
        }

        private void InitRechercheChemin()
        {
            RechercheChemin = new RechercheChemin((int)carte.Width, (int)carte.Height, Constante.TAILLE_CELLULE_RECHERCHE_CHEMIN);
            RechercheChemin.Grille.AjouterEntiteDansGrille(Ile);

            foreach (Obstacle obstacle in Obstacles)
            {
                RechercheChemin.Grille.AjouterEntiteDansGrille(obstacle);
            }
        }

        private void InitBoucleJeu()
        {
            CompositionTarget.Rendering += Jeu;
        }

        private void Jeu(object? sender, EventArgs e)
        {
            Joueur.Deplacer();

            DeplacerBoulets();
            CheckBouletsCollisions();

            if (Joueur.CanonActif)
                Joueur.TirerBoulet();

            if (Joueur.ModeTriche)
                Joueur.AjouterExperience(10000);
        }

        private void InitBonusMinuteur()
        {
            objetBonusMinuteur = new DispatcherTimer();
            objetBonusMinuteur.Interval = Constante.APPARITION_COFFRE_INTERVAL;
            objetBonusMinuteur.Tick += GenererBonus;
            objetBonusMinuteur.Start();
        }

        private void GenererBonus(object? sender, EventArgs e)
        {
            int randomQuantite = random.Next(Constante.BORNE_MIN_APPARITION_COFFRE, Constante.BORNE_MAX_APPARITION_COFFRE);
            int nbBonusAajouter = Math.Min(randomQuantite, Constante.BORNE_MAX_APPARITION_COFFRE - ObjetsBonus.Count);

            for (int i = 0; i < nbBonusAajouter; i++)
            {                
                double multiplicateur = Constante.MULTIPLICATEUR_TAILLE_COFFRE + random.NextDouble();
                int objetLargeur = (int)(Constante.BASE_COFFRE_LARGEUR * multiplicateur);
                int objetHauteur = (int)(Constante.BASE_COFFRE_HAUTEUR * multiplicateur);

                double posX, posY;
                PositionAleatoireValide(objetLargeur, objetHauteur, 0, out posX, out posY);

                TypeRecompense typeRecompense = (TypeRecompense)random.Next(0, Enum.GetValues(typeof(TypeRecompense)).Length);

                int valeurRecompense = 0;
                switch (typeRecompense)
                {
                    case TypeRecompense.VIE:
                        valeurRecompense = (int)(Constante.BASE_COFFRE_VIE * multiplicateur);
                        break;
                    case TypeRecompense.EXPERIENCE:
                        valeurRecompense = (int)(Constante.BASE_COFFRE_EXPERIENCE * multiplicateur);
                        break;
                }

                ObjetRecompense objet = new ObjetRecompense
                    (carte, bitmapTresor, objetLargeur, objetHauteur, valeurRecompense, typeRecompense, Constante.BASE_COFFRE_VIE, true);
                objet.Apparaitre(posX, posY);
                ObjetsBonus.Add(objet);

                RechercheChemin.Grille.AjouterEntiteDansGrille(objet);
            }
        }

        private bool CheckPositionValide(Collision collision)
        {
            if (Ile.EnCollisionAvec(collision))
            {
                return false;
            }

            foreach (Obstacle obstacleDejaPresent in Obstacles)
            {
                if (obstacleDejaPresent != null && obstacleDejaPresent.EnCollisionAvec(collision))
                {
                    return false;
                }
            }

            foreach (ObjetRecompense objetsBonusDejaPresent in ObjetsBonus)
            {
                if (objetsBonusDejaPresent != null && objetsBonusDejaPresent.EnCollisionAvec(collision))
                {
                    return false;
                }
            }

            return true;
        }

        private void PositionAleatoireValide(double largeur, double hauteur, double angleRotation, out double posX, out double posY)
        {
            posX = 0;
            posY = 0;

            bool positionValide;

            do
            {
                posX = random.Next(0, (int)(carte.Width - largeur));
                posY = random.Next(0, (int)(carte.Width - hauteur));

                Collision collision = new Collision(posX, posY, largeur, hauteur, angleRotation);

                positionValide = CheckPositionValide(collision);
            } while (!positionValide);
        }

        private void DeplacerBoulets()
        {
            for (int i = 0; i < Boulets.Count; i++)
            {
                Boulet boulet = Boulets[i];
                double bouletX = Canvas.GetLeft(boulet.CanvaElement);
                double bouletY = Canvas.GetTop(boulet.CanvaElement);

                bouletX += boulet.Direction.X * Constante.VITESSE_BOULET;
                bouletY += boulet.Direction.Y * Constante.VITESSE_BOULET;

                Canvas.SetLeft(boulet.CanvaElement, bouletX);
                Canvas.SetTop(boulet.CanvaElement, bouletY);

                if (bouletX < 0 || bouletY < 0 ||
                    bouletX > carte.Width + boulet.CanvaElement.Width || bouletY > carte.Height + boulet.CanvaElement.Width)
                {
                    carte.Children.Remove(boulet.CanvaElement);
                    Boulets.RemoveAt(i);
                }
            }

            if (Joueur.TempsDernierTir > 0)
            {
                Joueur.TempsDernierTir -= 1.0 / 60.0;
            }
        }

        private void CheckBouletsCollisions()
        {
            // Collision entre les boulets et les obstacles
            for (int i = Boulets.Count - 1; i >= 0; i--)
            {
                Boulet boulet = Boulets[i];
                foreach (Obstacle obstacle in Obstacles)
                {
                    if (boulet.EnCollisionAvec(obstacle))
                    {
                        Console.WriteLine("Collision avec un obstacle: X=" + obstacle.PositionX + " Y=" + obstacle.PositionY);

                        boulet.Disparaitre();
                        Boulets.RemoveAt(i);
                    }
                }

                if (boulet.EnCollisionAvec(Ile))
                {
                    boulet.Disparaitre();
                    Boulets.RemoveAt(i);
                }

                for (int j = ObjetsBonus.Count - 1; j >= 0; j--)
                {
                    ObjetRecompense objetBonus = ObjetsBonus[j];
                    if (boulet.EnCollisionAvec(objetBonus))
                    {
                        Console.WriteLine("Collision avec un objet bonus: X=" + objetBonus.PositionX + " Y=" + objetBonus.PositionY);
                        bool estDetruit = objetBonus.InfligerDegats(Joueur.Degats);

                        if (estDetruit && boulet.Tireur is Joueur)
                        {
                            switch (objetBonus.Type)
                            {
                                case TypeRecompense.VIE:
                                    Joueur.AjouterVie(objetBonus.ValeurRecompense);
                                    break;
                                case TypeRecompense.EXPERIENCE:
                                    Joueur.AjouterExperience(objetBonus.ValeurRecompense);
                                    break;
                            }

                            ObjetsBonus.RemoveAt(j);
                            RechercheChemin.Grille.SupprimerEntiteDansGrille(objetBonus);
                        }

                        boulet.Disparaitre();
                        Boulets.RemoveAt(i);
                    }
                }
            }
        }

        public void AjouterEnnemis(int nombreEnnemis, int vie, int degats, int vitesse, double rechargementCanon)
        {
            for (int i = 0; i < nombreEnnemis; i++)
            {
                Ennemi ennemi = new Ennemi(
                        carte,
                        this,
                        bitmapBateau,
                        vie,
                        degats,
                        vitesse,
                        rechargementCanon,
                        Ile
                    );

                double posX, posY;
                PositionAleatoireValide(ennemi.CanvaElement.Width, ennemi.CanvaElement.Height, 0, out posX, out posY);

                Ennemis.Add(ennemi);
                ennemi.Apparaitre(posX, posY);
            }
        }
    }
}
