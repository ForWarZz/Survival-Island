using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Survival_Island.Joueurs;
using Survival_Island.carte.objets;
using Survival_Island.carte;
using Survival_Island.Outils;
using Survival_Island.Carte.Objets;
using Survival_Island.Outils.Entites;

namespace Survival_Island
{
    public class MoteurJeu
    {
        public MainWindow fenetre { get; }

        private Canvas carte;
        private Image[] mer;

        private BitmapImage bitmapMer, bitmapTresor, bitmapBateau;
        private BitmapImage[] bitmapRochers;

        private DispatcherTimer objetBonusMinuteur;

        public Joueur joueur;
        private Random random;

        private bool incrementTempsEnDeplacement=false;

        public List<Boulet> boulets { get; }
        public Obstacle[] obstacles { get; }
        public List<ObjetRecompense> objetsBonus { get; }

        public Ile ile { get; private set; }

        public DispatcherTimer MinuteurDeplacement;

        public int tempsEnDeplacement { get; set; }


        public bool GodMod = false;
        public MoteurJeu(MainWindow fenetre)
        {
            this.fenetre = fenetre;

            carte = fenetre.carte;
            random = new Random();
            tempsEnDeplacement = 0;

            boulets = new List<Boulet>();
            objetsBonus = new List<ObjetRecompense>();
            obstacles = new Obstacle[Constante.NOMBRE_ROCHERS_CARTE];

            InitBitmaps();
            InitCarte();
        }

        public void InitJeu()
        {
            InitIle();
            InitRochers();

            InitJoueur();

            fenetre.hudJoueur.Visibility = Visibility.Visible;
            fenetre.btnAmeliorations.Visibility = Visibility.Visible;

            InitBonusMinuteur();
            InitBoucleJeu();

            InitMinuteurDeplacement();
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
            ile = new Ile(carte, fenetre, this);
            ile.Apparaitre();
        }

        private void InitJoueur()
        {
            double posX, posY;
            bool positionValide;

            do
            {
                posX = random.Next(0, (int)(carte.Width - Constante.LARGEUR_NAVIRE));
                posY = random.Next(0, (int)(carte.Height - Constante.HAUTEUR_NAVIRE));

                Collision collision = new Collision(posX, posY, Constante.LARGEUR_NAVIRE, Constante.HAUTEUR_NAVIRE);

                positionValide = CheckPositionValide(collision);
            } while (!positionValide);

            joueur = new Joueur(carte, this, bitmapBateau);
            joueur.Apparaitre(posX, posY);
        }

        private void InitRochers()
        {
            for (int i = 0; i < obstacles.Length; i++)
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
                bool positionValide;

                do
                {
                    posX = random.Next(0, (int)(carte.Width - rocher.Width));
                    posY = random.Next(0, (int)(carte.Width - rocher.Height));

                    Collision collision = new Collision(posX, posY, rocher.Width, rocher.Height, angleRotation);

                    positionValide = CheckPositionValide(collision);
                } while (!positionValide);

                Obstacle obstacle = new Obstacle(carte, rocher);
                obstacle.Apparaitre(posX, posY);

                obstacles[i] = obstacle;
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
            CheckDeplacement();
            DeplacerBoulets();

            CheckBouletsCollisions();

            if (joueur.canonActif)
                joueur.TirerBoulet();

            if (GodMod)
            {
                joueur.AjouterExperience(10000);
            }
        }

        private void CheckDeplacement()
        {
            double vitesse = joueur.vitesse;
            if (joueur.deplacement)
            {
                double accel = (vitesse / Constante.ACCELERATION) * tempsEnDeplacement;
                if ( accel < Constante.VITESSE_MAX)
                    incrementTempsEnDeplacement = true;
                else
                    incrementTempsEnDeplacement = false;
                vitesse = accel;

            }
            else
            {
                incrementTempsEnDeplacement = true;
                double decel = (vitesse / Constante.ACCELERATION) * tempsEnDeplacement;
                if (decel > 0)
                    vitesse = decel;
            }

            if (tempsEnDeplacement > 0)
            {
                Vector orientation = joueur.orientation;
                joueur.Deplacer(orientation.X * vitesse, orientation.Y * vitesse);
            }
        }

        private void InitMinuteurDeplacement()
        {
            MinuteurDeplacement = new DispatcherTimer();
            MinuteurDeplacement.Interval = Constante.VITESSE_ACCELERATION;
            MinuteurDeplacement.Tick += MinutDeplacement;
            MinuteurDeplacement.Start();
        }

        private void MinutDeplacement(object? sender, EventArgs e)
        {
            if (incrementTempsEnDeplacement)
            {
                if (joueur.deplacement)
                    tempsEnDeplacement += 1;
                else if (tempsEnDeplacement > 0)
                    tempsEnDeplacement -= 1;
            }
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
            int nbBonusAajouter = Math.Min(randomQuantite, Constante.BORNE_MAX_APPARITION_COFFRE - objetsBonus.Count);

            for (int i = 0; i < nbBonusAajouter; i++)
            {
                bool positionValide = true;
                double posX, posY;

                do
                {
                    posX = random.Next(0, (int)(carte.Width - Constante.BASE_COFFRE_LARGEUR));
                    posY = random.Next(0, (int)(carte.Height - Constante.BASE_COFFRE_HAUTEUR));

                    Collision collision = new Collision(posX, posY, Constante.BASE_COFFRE_LARGEUR, Constante.BASE_COFFRE_HAUTEUR);

                    positionValide = CheckPositionValide(collision);
                } while (!positionValide);

                double multiplicateur = Constante.MULTIPLICATEUR_TAILLE_COFFRE + random.NextDouble();
                int objetLargeur = (int)(Constante.BASE_COFFRE_LARGEUR * multiplicateur);
                int objetHauteur = (int)(Constante.BASE_COFFRE_HAUTEUR * multiplicateur);

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
                objetsBonus.Add(objet);
            }
        }

        private bool CheckPositionValide(Collision collision)
        {
            if (ile.EnCollisionAvec(collision))
            {
                return false;
            }

            foreach (Obstacle obstacleDejaPresent in obstacles)
            {
                if (obstacleDejaPresent != null && obstacleDejaPresent.EnCollisionAvec(collision))
                {
                    return false;
                }
            }

            foreach (ObjetRecompense objetsBonusDejaPresent in objetsBonus)
            {
                if (objetsBonusDejaPresent != null && objetsBonusDejaPresent.EnCollisionAvec(collision))
                {
                    return false;
                }
            }

            return true;
        }

        private void DeplacerBoulets()
        {
            for (int i = 0; i < boulets.Count; i++)
            {
                Boulet boulet = boulets[i];
                double bouletX = Canvas.GetLeft(boulet.canvaElement);
                double bouletY = Canvas.GetTop(boulet.canvaElement);

                bouletX += boulet.direction.X * Constante.VITESSE_BOULET;
                bouletY += boulet.direction.Y * Constante.VITESSE_BOULET;

                Canvas.SetLeft(boulet.canvaElement, bouletX);
                Canvas.SetTop(boulet.canvaElement, bouletY);

                if (bouletX < 0 || bouletY < 0 ||
                    bouletX > carte.Width + boulet.canvaElement.Width || bouletY > carte.Height + boulet.canvaElement.Width)
                {
                    carte.Children.Remove(boulet.canvaElement);
                    boulets.RemoveAt(i);
                }
            }

            if (joueur.tempsDernierTir > 0)
            {
                joueur.tempsDernierTir -= 1.0 / 60.0;
            }
        }

        private void CheckBouletsCollisions()
        {
            // Collision entre les boulets et les obstacles
            for (int i = boulets.Count - 1; i >= 0; i--)
            {
                Boulet boulet = boulets[i];
                foreach (Obstacle obstacle in obstacles)
                {
                    if (boulet.EnCollisionAvec(obstacle))
                    {
                        Console.WriteLine("Collision avec un obstacle: X=" + obstacle.PositionX + " Y=" + obstacle.PositionY);

                        boulet.Disparaitre();
                        boulets.RemoveAt(i);
                    }
                }

                if (boulet.EnCollisionAvec(ile))
                {
                    boulet.Disparaitre();
                    boulets.RemoveAt(i);
                }

                for (int j = objetsBonus.Count - 1; j >= 0; j--)
                {
                    ObjetRecompense objetBonus = objetsBonus[j];
                    if (boulet.EnCollisionAvec(objetBonus))
                    {
                        Console.WriteLine("Collision avec un objet bonus: X=" + objetBonus.PositionX + " Y=" + objetBonus.PositionY);
                        bool estDetruit = objetBonus.InfligerDegats(joueur.degats);

                        if (estDetruit && boulet.tireur is Joueur)
                        {
                            switch (objetBonus.type)
                            {
                                case TypeRecompense.VIE:
                                    joueur.AjouterVie(objetBonus.valeurRecompense);
                                    break;
                                case TypeRecompense.EXPERIENCE:
                                    joueur.AjouterExperience(objetBonus.valeurRecompense);
                                    break;
                            }

                            objetsBonus.RemoveAt(j);
                        }

                        boulet.Disparaitre();
                        boulets.RemoveAt(i);
                    }
                }
            }
        }

        public void JoueurTire(bool canonActif)
        {
            joueur.canonActif = canonActif;
        }

        public void JoueurOriente(Point positionSouris)
        {
            joueur.ChangerOrientation(positionSouris);
        }

        public void JoueurDeplace(bool deplacement)
        {
            joueur.deplacement = deplacement;
        }
    }
}
