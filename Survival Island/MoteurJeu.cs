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
        public bool EstPause { get; private set; }
        public bool PauseActive { get; private set; }

        private Canvas carte;
        private Image[] mer;

        private BitmapImage bitmapMer, bitmapTresor, bitmapBateau, bitmapBateauEnnemi;
        private BitmapImage[] bitmapRochers;

        private DispatcherTimer objetBonusMinuteur;

        public Joueur Joueur { get; private set; }
        public Ile Ile { get; private set; }
        public GestionVagues GestionVagues { get; set; }

        private Random random;

        public List<Boulet> Boulets { get; }
        public Obstacle[] Obstacles { get; }
        public List<ObjetRecompense> ObjetsBonus { get; }

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

            Joueur = new Joueur(carte, this, bitmapBateau);
        }

        public void InitJeu()
        {
            InitIle();
            InitGestionnaireVague();

            InitRochers();
            InitJoueur();

            InitBonusMinuteur();
            InitBoucleJeu();

            AfficherHUD(true);
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

            bitmapBateauEnnemi = new BitmapImage(new Uri(Chemin.IMAGE_BATEAU_ENNEMI));
        }

        private void InitIle()
        {
            Ile = new Ile(carte, Fenetre, this);
            Ile.Apparaitre();
        }

        private void InitJoueur()
        {
            Point position = GenererPositionAleatoire(Constante.LARGEUR_NAVIRE, Constante.HAUTEUR_NAVIRE, 0);
            Joueur.Apparaitre(position);
        }

        private void InitGestionnaireVague()
        {
            GestionVagues = new GestionVagues(carte, this, bitmapBateauEnnemi);
            GestionVagues.LancerMinuteurVague();
        }

        private void InitRochers()
        {
            for (int i = 0; i < Obstacles.Length; i++)
            {
                Image rocher = new Image();
                BitmapImage randomRocher = bitmapRochers[random.Next(0, bitmapRochers.Length)];

                double angleRotation = random.Next(0, 361);
                double multiplicateurTaille = Constante.MULTIPLICATEUR_TAILLE_ROCHER + random.NextDouble();

                rocher.Source = randomRocher;
                rocher.Width = randomRocher.Width * multiplicateurTaille;
                rocher.Height = randomRocher.Height * multiplicateurTaille;
                rocher.RenderTransform = new RotateTransform(angleRotation, rocher.Width / 2, rocher.Height / 2);

                Point position = GenererPositionAleatoire(rocher.Width, rocher.Height, angleRotation);

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
            if (EstPause) return;

            DateTime tempsActuel = DateTime.Now;
            TimeSpan tempsEcoule = tempsActuel - miseAJourTemps;
            double deltaTemps = tempsEcoule.TotalSeconds;

            miseAJourTemps = tempsActuel;

            Joueur.Deplacer(deltaTemps);

            foreach (Ennemi ennemi in GestionVagues.EnnemisActuels)
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

            if (Joueur.TempsDernierTir > 0)
                Joueur.TempsDernierTir -= deltaTemps;

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

                Point position = GenererPositionAleatoire(objetLargeur, objetHauteur, 0);

                int valeurExperience = (int)(Constante.BASE_COFFRE_EXPERIENCE * multiplicateur);

                int valeurVie = 0;
                if (random.NextDouble() < Constante.BASE_COFFRE_PROBA_VIE)
                {
                    valeurVie = (int)(Constante.BASE_COFFRE_VIE * multiplicateur);
                }

                ObjetRecompense objet = new ObjetRecompense
                    (carte, bitmapTresor, objetLargeur, objetHauteur, valeurExperience, valeurVie, Constante.BASE_COFFRE_VIE);
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

            foreach (Ennemi ennemi in GestionVagues.EnnemisActuels)
            {
                if (ennemi.EnCollisionAvec(collision))
                {
                    return false;
                }
            }

            return true;
        }

        public Point GenererPositionAleatoire(double largeur, double hauteur, double angleRotation = 0, int marge = 0)
        {
            if (marge < 0)
                throw new ArgumentException("La marge doit être positive.");

            if (marge > 0 && (marge > carte.Width || marge > carte.Height))
                throw new ArgumentException("La marge est plus grande que la taille de la carte.");

            double posX, posY;
            bool positionValide;

            int carteLargeur = (int)carte.Width;
            int carteHauteur = (int)carte.Height;

            do
            {
                if (marge > 0)
                {
                    // Générer la position sur un bord en tenant compte de la marge
                    int bord = random.Next(0, 4); // 0 = gauche, 1 = droite, 2 = haut, 3 = bas

                    switch (bord)
                    {
                        case 0: // Bord gauche
                            posX = random.Next(0, marge);
                            posY = random.Next(0, carteHauteur - (int)hauteur);
                            break;
                        case 1: // Bord droit
                            posX = random.Next(carteLargeur - marge, carteLargeur - (int)largeur);
                            posY = random.Next(0, carteHauteur - (int)hauteur);
                            break;
                        case 2: // Bord haut
                            posX = random.Next(0, carteLargeur - (int)largeur);
                            posY = random.Next(0, marge);
                            break;
                        default: // Bord bas
                            posX = random.Next(0, carteLargeur - (int)largeur);
                            posY = random.Next(carteHauteur - marge, carteHauteur - (int)hauteur);
                            break;
                    }
                }
                else
                {
                    posX = random.Next(0, carteLargeur - (int)largeur);
                    posY = random.Next(0, carteHauteur - (int)hauteur);
                }

                // Vérifier la validité de la position
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
        }

        private void CheckBouletsCollisions()
        {
            // Liste des boulets à supprimer
            List<Boulet> bouletsASupprimer = new List<Boulet>();

            // Parcourir tous les boulets
            for (int i = Boulets.Count - 1; i >= 0; i--)
            {
                Boulet boulet = Boulets[i];

                // Collision entre le boulet et les obstacles
                foreach (Obstacle obstacle in Obstacles)
                {
                    if (boulet.EnCollisionAvec(obstacle))
                    {
                        bouletsASupprimer.Add(boulet);
                        break;
                    }
                }

                // Collision boulet avec l'île
                if (boulet.EnCollisionAvec(Ile))
                {
                    if (boulet.Tireur is Ennemi)
                    {
                        Ile.InfligerDegats(boulet.Tireur.Degats);
                    }

                    bouletsASupprimer.Add(boulet);
                    continue;
                }

                // Collision entre boulet et objets bonus
                for (int j = ObjetsBonus.Count - 1; j >= 0; j--)
                {
                    ObjetRecompense objetBonus = ObjetsBonus[j];
                    if (boulet.EnCollisionAvec(objetBonus))
                    {
                        bool estDetruit = objetBonus.InfligerDegats(Joueur.Degats);

                        if (estDetruit && boulet.Tireur is Joueur)
                        {
                            Joueur.AjouterExperience(objetBonus.RecompenseExperience);
                            Joueur.AjouterVie(objetBonus.RecompenseVie);

                            ObjetsBonus.RemoveAt(j);
                        }

                        bouletsASupprimer.Add(boulet);
                        break;
                    }
                }

                // Collision entre boulet ennemi et joueur
                if (Joueur.EnCollisionAvec(boulet) && boulet.Tireur is Ennemi)
                {
                    Joueur.InfligerDegats(boulet.Tireur.Degats);
                    bouletsASupprimer.Add(boulet);
                    continue;
                }

                // Collision entre boulet joueur et ennemis
                for (int j = GestionVagues.EnnemisActuels.Count - 1; j >= 0; j--)
                {
                    Ennemi ennemi = GestionVagues.EnnemisActuels[j];
                    if (ennemi.EnCollisionAvec(boulet) && boulet.Tireur is Joueur)
                    {
                        bool mort = ennemi.InfligerDegats(Joueur.Degats);

                        if (mort)
                        {
                            GestionVagues.EnnemisActuels.RemoveAt(j);
                            GestionVagues.MettreAJour();

                            Joueur.NombreTue++;
                            Joueur.MettreAJour();
                        }

                        bouletsASupprimer.Add(boulet);
                        break;
                    }
                }
            }

            // Supprimer les boulets marqués
            foreach (Boulet boulet in bouletsASupprimer)
            {
                boulet.Disparaitre();
                Boulets.Remove(boulet);
            }
        }

        public void TerminerJeu()
        {
            GestionVagues.MinuteurVague.Stop();
            objetBonusMinuteur.Stop();
            EstPause = true;

            AfficherHUD(false);

            Fenetre.spMenuFin.Visibility = Visibility.Visible;
        }

        public void Pause()
        {
            if (EstPause)
            {
                AfficherHUD(true);
                EstPause = false;

                GestionVagues.MinuteurVague.Start();
                objetBonusMinuteur.Start();
            }
            else
            {
                AfficherHUD(false);
                EstPause = true;

                GestionVagues.MinuteurVague.Stop();
                objetBonusMinuteur.Stop();
            }
        }

        public void Rejouer()
        {
            carte.Children.Clear();
            Boulets.Clear();
            ObjetsBonus.Clear();

            InitJeu();
        }

        private void AfficherHUD(bool afficher)
        {
            if (afficher)
            {
                Fenetre.hudJoueur.Visibility = Visibility.Visible;
                Fenetre.gridBoutonAmelio.Visibility = Visibility.Visible;

                Fenetre.txtNombreTue.Visibility = Visibility.Visible;
                Fenetre.txtVagueActuelle.Visibility = Visibility.Visible;

                Fenetre.txtStatusVague.Visibility = Visibility.Visible;
            }
            else
            {
                Fenetre.hudJoueur.Visibility = Visibility.Hidden;
                Fenetre.gridBoutonAmelio.Visibility = Visibility.Hidden;

                Fenetre.txtNombreTue.Visibility = Visibility.Hidden;
                Fenetre.txtVagueActuelle.Visibility = Visibility.Hidden;

                Fenetre.txtStatusVague.Visibility = Visibility.Hidden;
            }
        }
    }
}
