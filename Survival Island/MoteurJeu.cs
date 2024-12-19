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
        public Canvas Carte { get; }

        public bool EstPause { get; private set; }
        public bool JeuLance { get; set; }


        private DispatcherTimer objetBonusMinuteur;

        public Joueur Joueur { get; private set; }

        public GestionImages GestionImages { get; }
        public GestionSons GestionSons { get; }

        public GestionVagues GestionVagues { get; private set; }
        public GestionCarte GestionCarte { get; private set; }

        private Random random;

        public List<Boulet> Boulets { get; }
        public List<ObjetRecompense> ObjetsBonus { get; }

        public int NumBateau { get; set; }
        
        private DateTime miseAJourTemps;

        public MoteurJeu(MainWindow fenetre)
        {
            Fenetre = fenetre;

            EstPause = false;
            JeuLance = false;

            Carte = fenetre.carte;
            random = new Random();

            NumBateau = 0;

            Boulets = new List<Boulet>();
            ObjetsBonus = new List<ObjetRecompense>();

            GestionImages = new GestionImages();
            GestionCarte = new GestionCarte(this);

            GestionSons = new GestionSons();

            Joueur = new Joueur(this);
        }

        public void InitJeu()
        {
            JeuLance = true;

            GestionCarte.InitIle();

            GestionVagues = new GestionVagues(this);
            GestionCarte.InitRochers();

            InitJoueur();

            InitBonusMinuteur();
            InitBoucleJeu();

            AfficherHUD(true);

            GestionVagues.LancerMinuteurVague();
        }

        private void InitJoueur()
        {
            Point position = GestionCarte.GenererPositionAleatoire(Constante.LARGEUR_NAVIRE, Constante.HAUTEUR_NAVIRE, 0);
            Joueur.Apparaitre(position);
        }

        private void InitBoucleJeu()
        {
            if (EstPause) return;

            CompositionTarget.Rendering += Jeu;
        }

        private void Jeu(object? sender, EventArgs e)
        {
            if (EstPause) return;

            /*Permet de calculer le temps écoulé entre chaque mise à jour, afin de garantir une stabilité *
              des mouvements et autres actions, sans dépendre du materiel. (Non utilisation de dispatcher, afin d'avoir un jeu plus fluide,
              mais necessite ce calcul, pour */
            DateTime tempsActuel = DateTime.Now;
            TimeSpan tempsEcoule = tempsActuel - miseAJourTemps;
            double deltaTemps = tempsEcoule.TotalSeconds;

            miseAJourTemps = tempsActuel;

            Joueur.Deplacer(deltaTemps);
            Joueur.MiseAJourRotation(deltaTemps);

            foreach (Ennemi ennemi in GestionVagues.EnnemisActuels)
            {
                ennemi.Deplacer(deltaTemps);
                ennemi.MiseAJourRotation(deltaTemps);
                ennemi.VerifierJoueursDansRayon();

                if (ennemi.CanonActif)
                    ennemi.TirerBoulet();

                if (ennemi.TempsDernierTir > 0)
                    ennemi.TempsDernierTir -= deltaTemps;
            }

            DeplacerBoulets(deltaTemps);
            CheckBouletsCollisions();

            if (Joueur.CanonActif && !Joueur.EstMort)
                Joueur.TirerBoulet();

            if (Joueur.TempsDernierTir > 0)
                Joueur.TempsDernierTir -= deltaTemps;

            if (Joueur.ModeTriche)
                Joueur.AjouterExperience(Constante.EXPERIENCE_MODE_TRICHE);
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

                Point position = GestionCarte.GenererPositionAleatoire(objetLargeur, objetHauteur, 0);

                int valeurExperience = (int)(Constante.BASE_COFFRE_EXPERIENCE * multiplicateur);

                int valeurVie = 0;
                if (random.NextDouble() < Constante.BASE_COFFRE_PROBA_VIE)
                {
                    valeurVie = (int)(Constante.BASE_COFFRE_VIE * multiplicateur);
                }

                ObjetRecompense objet = new ObjetRecompense
                    (Carte, GestionImages.Tresor, objetLargeur, objetHauteur, valeurExperience, valeurVie, Constante.BASE_COFFRE_VIE);
                objet.Apparaitre(position);
                ObjetsBonus.Add(objet);
            }
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
                    bouletX > Carte.Width + boulet.CanvaElement.Width || bouletY > Carte.Height + boulet.CanvaElement.Width)
                {
                    Carte.Children.Remove(boulet.CanvaElement);
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
                foreach (Obstacle obstacle in GestionCarte.Obstacles)
                {
                    if (boulet.EnCollisionAvec(obstacle))
                    {
                        //Console.WriteLine("DEBUG: Boulet touché obstacle");

                        bouletsASupprimer.Add(boulet);
                        break;
                    }
                }

                // Collision boulet avec l'île
                if (boulet.EnCollisionAvec(GestionCarte.Ile))
                {
                    if (boulet.Tireur is Ennemi)
                    {
                        GestionCarte.Ile.InfligerDegats(boulet.Tireur.Degats);
                    }

                    //Console.WriteLine("DEBUG: Boulet touché l'île");

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

                        if (estDetruit)
                        {
                            if (boulet.Tireur is Joueur)
                            {
                                Joueur.AjouterExperience(objetBonus.RecompenseExperience);
                                Joueur.AjouterVie(objetBonus.RecompenseVie);
                            }

                            ObjetsBonus.RemoveAt(j);
                        }

                        //Console.WriteLine("DEBUG: Objet bonus touché");

                        bouletsASupprimer.Add(boulet);
                        break;
                    }
                }

                // Collision entre boulet ennemi et joueur
                if (Joueur.EnCollisionAvec(boulet) && !Joueur.EstMort && boulet.Tireur is Ennemi)
                {
                    Joueur.InfligerDegats(boulet.Tireur.Degats);
                    bouletsASupprimer.Add(boulet);

                    //Console.WriteLine("DEBUG: Joueur touché");

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

                            Joueur.NombreCoule++;
                            Joueur.AjouterExperience(Constante.RECOMPENSE_EXP_ENNEMI_TUE);
                            Joueur.MettreAJour();
                        }

                        //Console.WriteLine("DEBUG: Ennemi touché");

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
            Joueur.MinuteurReapparition.Stop();

            EstPause = true;

            AfficherHUD(false);

            Fenetre.txtNbMorts.Text = Joueur.NombreMort.ToString();
            Fenetre.txtNbCoules.Text = Joueur.NombreCoule.ToString();
            Fenetre.txtNbVagues.Text = (GestionVagues.NumeroVague - 1).ToString();

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
            GestionCarte.Ile.Disparaitre();
            Joueur.Disparaitre();

            for (int i = Boulets.Count - 1; i >= 0; i--)
            {
                Boulets[i].Disparaitre();
                Boulets.RemoveAt(i);
            }

            for (int i = ObjetsBonus.Count - 1; i >= 0; i--)
            {
                ObjetsBonus[i].Disparaitre();
                ObjetsBonus.RemoveAt(i);
            }

            for (int i = GestionCarte.Obstacles.Length - 1; i >= 0; i--)
            {
                GestionCarte.Obstacles[i].Disparaitre();
            }

            for (int i = GestionVagues.EnnemisActuels.Count - 1; i >= 0; i--)
            {
                GestionVagues.EnnemisActuels[i].Disparaitre();
                GestionVagues.EnnemisActuels.RemoveAt(i);
            }

            Joueur nouveauJoueur = new Joueur(this);
            ((Image)nouveauJoueur.CanvaElement).Source = ((Image)Joueur.CanvaElement).Source;

            Joueur = nouveauJoueur;
            InitJeu();

            EstPause = false;
        }

        private void AfficherHUD(bool afficher)
        {
            if (afficher)
            {
                Fenetre.hudJoueur.Visibility = Visibility.Visible;

                Fenetre.spAmelio.Visibility = Fenetre.menuActif ? Visibility.Visible : Visibility.Hidden;
                Fenetre.gridBoutonAmelio.Visibility = Visibility.Visible;

                Fenetre.spJoueurStats.Visibility = Visibility.Visible;
                Fenetre.spVagueInfo.Visibility = Visibility.Visible;

                Fenetre.txtStatusVague.Visibility = Visibility.Visible;
            }
            else
            {
                Fenetre.hudJoueur.Visibility = Visibility.Hidden;

                Fenetre.spAmelio.Visibility = Visibility.Hidden;
                Fenetre.gridBoutonAmelio.Visibility = Visibility.Hidden;

                Fenetre.spJoueurStats.Visibility = Visibility.Hidden;
                Fenetre.spVagueInfo.Visibility = Visibility.Hidden;

                Fenetre.txtStatusVague.Visibility = Visibility.Hidden;
            }
        }
    }
}
