using Survival_Island.Entites;
using Survival_Island.Entites.Base;
using Survival_Island.Entites.Navire;
using Survival_Island.Entites.Objets;
using Survival_Island.Outils;
using System.IO;
using System.Media;
using System.Threading;
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

        public bool JeuTermine { get; set; }

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
        private GestionVagues gestionVagues { get; set; }

        public int NumBateau { get; set; }

        public MediaPlayer MediaPlayerMusique { get; private set; }
        public SoundPlayer SoundPlayerTire { get; private set; }
        
        private DateTime miseAJourTemps;

        public MoteurJeu(MainWindow fenetre)
        {
            Fenetre = fenetre;

            carte = fenetre.carte;
            random = new Random();

            NumBateau = 0;

            Boulets = new List<Boulet>();
            ObjetsBonus = new List<ObjetRecompense>();
            Obstacles = new Obstacle[Constante.NOMBRE_ROCHERS_CARTE];

            InitSons();
            InitBitmaps();
            InitCarte();
        }

        public void InitJeu()
        {
            InitIle();
            InitRochers();

            InitJoueur();

            InitBonusMinuteur();
            InitBoucleJeu();

            gestionVagues = new GestionVagues(carte, this, bitmapBateau);
            gestionVagues.LancerMinuteurVague();
        }

        private void InitSons()
        {
            MediaPlayerMusique = new MediaPlayer();

            MediaPlayerMusique.Open(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Chemin.MUSIQUE_FOND)));
            MediaPlayerMusique.Volume = 0.2;
            MediaPlayerMusique.MediaEnded += (sender, e) => MediaPlayerMusique.Position = TimeSpan.Zero;
            MediaPlayerMusique.Play();

            SoundPlayerTire = new SoundPlayer(Application.GetResourceStream(new Uri(Chemin.SON_TIRE)).Stream);
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
            Point position = PositionAleatoireValide(Constante.LARGEUR_NAVIRE, Constante.HAUTEUR_NAVIRE, 0);

            Joueur = new Joueur(carte, this, bitmapBateau);
            Joueur.Apparaitre(position);
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

                Point position = PositionAleatoireValide(rocher.Width, rocher.Height, angleRotation);

                Obstacle obstacle = new Obstacle(carte, rocher);
                obstacle.Apparaitre(position);

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

        private void InitBoucleJeu()
        {
            CompositionTarget.Rendering += Jeu;
        }

        private void Jeu(object? sender, EventArgs e)
        {
            DateTime tempsActuel = DateTime.Now;
            TimeSpan tempsEcoule = tempsActuel - miseAJourTemps;
            double deltaTemps = tempsEcoule.TotalSeconds;

            miseAJourTemps = tempsActuel;

            Joueur.Deplacer(deltaTemps);

            foreach (Ennemi ennemi in gestionVagues.EnnemisActuels)
            {
                ennemi.Deplacer(deltaTemps);
                ennemi.VerifierJoueursDansRayon();

                if (ennemi.CanonActif)
                    ennemi.TirerBoulet();

                if (ennemi.TempsDernierTir > 0)
                    ennemi.TempsDernierTir -= deltaTemps;
            }

            DeplacerBoulets(deltaTemps);
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

                Point position = PositionAleatoireValide(objetLargeur, objetHauteur, 0);

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
                objet.Apparaitre(position);
                ObjetsBonus.Add(objet);
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

        public Point PositionAleatoireValide(double largeur, double hauteur, double angleRotation = 0)
        {
            double posX, posY;
            bool positionValide;

            do
            {
                posX = random.Next(0, (int)(carte.Width - largeur));
                posY = random.Next(0, (int)(carte.Height - hauteur));

                Collision collision = new Collision(posX, posY, largeur, hauteur, angleRotation);
                positionValide = CheckPositionValide(collision);
            } while (!positionValide);

            return new Point(posX, posY);
        }

        public Point PositionAleatoireValide(double largeur, double hauteur, double angleRotation = 0, int marge = 0)
        {
            double posX, posY;
            bool positionValide;

            if (marge > carte.Width || marge > carte.Height)
                throw new ArgumentException("La marge est plus grande que la carte.");

            int carteLargeur = (int)carte.Width;
            int carteHauteur = (int)carte.Height;

            do
            {
                // Choisir aléatoirement sur quel bord générer la position (gauche, droite, haut ou bas)
                int bord = random.Next(0, 4); // 0 = gauche, 1 = droite, 2 = haut, 3 = bas

                if (bord == 0) // Bord gauche
                {
                    posX = random.Next(0, marge); // X entre 0 et la marge
                    posY = random.Next(0, carteHauteur - (int)hauteur); // Y dans toute la hauteur de la carte, mais en tenant compte de la hauteur du bateau
                }
                else if (bord == 1) // Bord droit
                {
                    posX = random.Next(carteLargeur - marge, carteLargeur - (int)(largeur)); // X entre (taille carte - marge) et la taille de la carte
                    posY = random.Next(0, carteHauteur - (int)hauteur); // Y dans toute la hauteur de la carte, mais en tenant compte de la hauteur du bateau
                }
                else if (bord == 2) // Bord haut
                {
                    posX = random.Next(0, carteLargeur - (int)largeur); // X dans toute la largeur de la carte, mais en tenant compte de la largeur du bateau
                    posY = random.Next(0, marge); // Y entre 0 et la marge
                }
                else // Bord bas
                {
                    posX = random.Next(0, carteLargeur - (int)largeur); // X dans toute la largeur de la carte, mais en tenant compte de la largeur du bateau
                    posY = random.Next(carteHauteur - marge, carteHauteur - (int)hauteur); // Y entre (taille carte - marge) et la taille de la carte
                }

                // Créer un objet de collision avec la position et vérifier sa validité
                Collision collision = new Collision(posX, posY, largeur, hauteur, angleRotation);
                positionValide = CheckPositionValide(collision);
            } while (!positionValide);

            return new Point(posX, posY);
        }

        private void DeplacerBoulets(double deltaTemps)
        {
            for (int i = 0; i < Boulets.Count; i++)
            {
                Boulet boulet = Boulets[i];
                double bouletX = Canvas.GetLeft(boulet.CanvaElement);
                double bouletY = Canvas.GetTop(boulet.CanvaElement);

                bouletX += boulet.Direction.X * Constante.VITESSE_BOULET * deltaTemps;
                bouletY += boulet.Direction.Y * Constante.VITESSE_BOULET * deltaTemps;

                Canvas.SetLeft(boulet.CanvaElement, bouletX);
                Canvas.SetTop(boulet.CanvaElement, bouletY);

                // Supprimer les boulets qui sortent de la carte
                if (bouletX < 0 || bouletY < 0 ||
                    bouletX > carte.Width + boulet.CanvaElement.Width || bouletY > carte.Height + boulet.CanvaElement.Width)
                {
                    carte.Children.Remove(boulet.CanvaElement);
                    Boulets.RemoveAt(i);
                }
            }

            if (Joueur.TempsDernierTir > 0)
            {
                Joueur.TempsDernierTir -= deltaTemps;
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
                        boulet.Disparaitre();
                        Boulets.RemoveAt(i);
                    }
                }

                // Collision boulet avec ile
                if (boulet.EnCollisionAvec(Ile))
                {
                    if (boulet.Tireur is Ennemi)
                    {
                        Ile.InfligerDegats(boulet.Tireur.Degats);
                    }

                    boulet.Disparaitre();
                    Boulets.RemoveAt(i);
                }

                // Collision boulets avec objets bonus
                for (int j = ObjetsBonus.Count - 1; j >= 0; j--)
                {
                    ObjetRecompense objetBonus = ObjetsBonus[j];
                    if (boulet.EnCollisionAvec(objetBonus))
                    {
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
                        }

                        boulet.Disparaitre();
                        Boulets.RemoveAt(i);
                    }
                }

                // Boulet ennemi sur joueur
                if (Joueur.EnCollisionAvec(boulet) && boulet.Tireur is Ennemi)
                {
                    Joueur.InfligerDegats(boulet.Tireur.Degats);

                    boulet.Disparaitre();
                    Boulets.RemoveAt(i);
                }

                // Boulets joueur sur ennemi
                for (int j = gestionVagues.EnnemisActuels.Count - 1; j >= 0; j--)
                {
                    Ennemi ennemi = gestionVagues.EnnemisActuels[j];
                    if (ennemi.EnCollisionAvec(boulet) && boulet.Tireur is Joueur)
                    {
                        bool mort = ennemi.InfligerDegats(Joueur.Degats);

                        if (mort)
                        {
                            gestionVagues.EnnemisActuels.RemoveAt(j);
                            gestionVagues.MettreAJour();

                            Joueur.NombreTue++;
                            Joueur.MettreAJour();
                        }

                        boulet.Disparaitre();
                        Boulets.RemoveAt(i);
                    }
                }
            }
        }
    }
}
