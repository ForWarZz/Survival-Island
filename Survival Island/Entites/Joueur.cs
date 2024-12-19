using Survival_Island.Entites.Base;
using Survival_Island.Entites.Navire;
using Survival_Island.Entites.Objets;
using Survival_Island.Outils;
using System.Media;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Survival_Island.Entites
{
    public class Joueur : Bateau
    {
        private MainWindow fenetre;
        private double cameraX, cameraY;

        public int Niveau { get; private set; }
        public int Experience { get; private set; }
        public int ExperienceMax { get; private set; }

        public int PointsAmeliorations { get; set; }
        public bool ModeTriche { get; set; }

        public int NiveauClasse { get; set; }
        public bool NouveauNiveau { get; set; }

        public int NombreCoule { get; set; }
        public int NombreMort { get; set; }


        public DispatcherTimer MinuteurReapparition { get; set; }

        public Joueur(MoteurJeu moteurJeu) :
            base(moteurJeu, moteurJeu.GestionImages.BateauxJoueur[0], Brushes.Black, 
                false, Constante.JOUEUR_VIE_MAX, Constante.JOUEUR_DEGATS, Constante.JOUEUR_VITESSE, 
                Constante.JOUEUR_RECHARGEMENT_CANON)
        {
            fenetre = moteurJeu.Fenetre;
            Niveau = 1;
            PointsAmeliorations = 0;
            Experience = 0;
            ExperienceMax = Constante.JOUEUR_EXPERIENCE_MAX_N1;

            NombreMort = 0;
            NombreCoule = 0;

            ModeTriche = false;

            NiveauClasse = 0;
            NouveauNiveau = false;

            InitReapparitionMinuteur();
        }

        private void InitReapparitionMinuteur()
        {
            MinuteurReapparition = new DispatcherTimer();
            MinuteurReapparition.Interval = Constante.TEMPS_REAPPARITION;
            MinuteurReapparition.Tick += Reapparition;
        }

        private void AfficherMenuClasse()
        {
            if (ModeBateau == Constante.MODE_CLASSIQUE_NOM)
            {
                //Affichage du menu de niveau 5

                fenetre.menuClasse.Visibility = Visibility.Visible;

                fenetre.btnQuatre.Click += (sender, e) => { ChoisirClasse(Constante.MODE_QUATRE_NOM); };
                fenetre.btnPompe.Click += (sender, e) => { ChoisirClasse(Constante.MODE_POMPE_NOM); };
                fenetre.btnDouble.Click += (sender, e) => { ChoisirClasse(Constante.MODE_DOUBLE_NOM); };
            }
            else if (ModeBateau == Constante.MODE_QUATRE_NOM)
            {
                fenetre.menuClasse.Visibility = Visibility.Visible;

                fenetre.imgModeDeTir1.Source = MoteurJeu.GestionImages.ImageOcto;
                fenetre.imgModeDeTir2.Source = MoteurJeu.GestionImages.ImageQuatroPlus;
                fenetre.imgModeDeTir3.Source = MoteurJeu.GestionImages.ImageEventaille;

                fenetre.btnQuatre.Content = Constante.MODE_OCTOPUS_NOM;
                fenetre.btnQuatre.Click += (sender, e) => { ChoisirClasse(Constante.MODE_OCTOPUS_NOM); };

                fenetre.btnPompe.Content = Constante.MODE_QUATRO_PLUS_NOM;
                fenetre.btnPompe.Click += (sender, e) => { ChoisirClasse(Constante.MODE_QUATRO_PLUS_NOM); };

                fenetre.btnDouble.Content = Constante.MODE_EVENTAILLE_NOM;
                fenetre.btnDouble.Click += (sender, e) => { ChoisirClasse(Constante.MODE_EVENTAILLE_NOM); };
            }
            else if (ModeBateau == Constante.MODE_DOUBLE_NOM)
            {
                fenetre.menuClasse.Visibility = Visibility.Visible;

                fenetre.imgModeDeTir1.Source = MoteurJeu.GestionImages.ImageTrio;
                fenetre.imgModeDeTir2.Source = MoteurJeu.GestionImages.ImageMega;
                fenetre.imgModeDeTir3.Source = MoteurJeu.GestionImages.ImageEventaille;

                fenetre.btnQuatre.Content = Constante.MODE_TRIO_NOM;
                fenetre.btnQuatre.Click += (sender, e) => { ChoisirClasse(Constante.MODE_TRIO_NOM); };

                fenetre.btnPompe.Content = Constante.MODE_MEGA_NOM;
                fenetre.btnPompe.Click += (sender, e) => { ChoisirClasse(Constante.MODE_MEGA_NOM); };

                fenetre.btnDouble.Content = Constante.MODE_EVENTAILLE_NOM;
                fenetre.btnDouble.Click += (sender, e) => { ChoisirClasse(Constante.MODE_EVENTAILLE_NOM); };
            }
            else if (ModeBateau == Constante.MODE_POMPE_NOM)
            {
                fenetre.menuClasse.Visibility = Visibility.Visible;

                fenetre.imgModeDeTir1.Source = MoteurJeu.GestionImages.ImageSniper;
                fenetre.imgModeDeTir2.Source = MoteurJeu.GestionImages.ImageMk30;
                fenetre.imgModeDeTir3.Source = MoteurJeu.GestionImages.ImageEventaille;

                fenetre.btnQuatre.Content = Constante.MODE_SNIPER_NOM;
                fenetre.btnQuatre.Click += (sender, e) => { ChoisirClasse(Constante.MODE_SNIPER_NOM); };

                fenetre.btnPompe.Content = Constante.MODE_MK30_NOM;
                fenetre.btnPompe.Click += (sender, e) => { ChoisirClasse(Constante.MODE_MK30_NOM); };

                fenetre.btnDouble.Content = Constante.MODE_EVENTAILLE_NOM;
                fenetre.btnDouble.Click += (sender, e) => { ChoisirClasse(Constante.MODE_EVENTAILLE_NOM); };
            }

            fenetre.btnIgnore.Click += (sender, e) => { ChoisirClasse(Constante.MODE_IGNORE_NOM); };
        }

        private void ChoisirClasse(string classe)
        {
            // Appliquer les modifications en fonction de la classe choisie
            if (Niveau >= 5 && ModeBateau == Constante.MODE_CLASSIQUE_NOM && classe != Constante.MODE_IGNORE_NOM)
            {
                if (classe == Constante.MODE_QUATRE_NOM)
                {
                    ModeBateau = Constante.MODE_QUATRE_NOM;
                    this.nombreBouletsParShoot = (int)Constante.MODE_QUATRE[0];
                    this.angleBoulets = (int)Constante.MODE_QUATRE[1];
                    this.TempsRechargementCanon = Constante.MODE_QUATRE[2];
                    this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;
                }
                else if (classe == Constante.MODE_DOUBLE_NOM)
                {
                    ModeBateau = Constante.MODE_DOUBLE_NOM;
                    this.nombreBouletsParShoot = (int)Constante.MODE_DOUBLE[0];
                    this.angleBoulets = (int)Constante.MODE_DOUBLE[1];
                    this.TempsRechargementCanon = Constante.MODE_DOUBLE[2];
                    this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;
                    this.espacementBoulets = (int)Constante.MODE_DOUBLE[3];
                }
                else if (classe == Constante.MODE_POMPE_NOM)
                {
                    ModeBateau = Constante.MODE_POMPE_NOM;
                    this.nombreBouletsParShoot = (int)Constante.MODE_POMPE[0];
                    this.angleBoulets = (int)Constante.MODE_POMPE[1];
                    this.TempsRechargementCanon = Constante.MODE_POMPE[2];
                    this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;
                }
                this.NiveauClasse = 1;
            }
            else if (Niveau >= 15 && classe != Constante.MODE_IGNORE_NOM)
            {
                if (ModeBateau == Constante.MODE_QUATRE_NOM)
                {
                    if (classe == Constante.MODE_OCTOPUS_NOM)
                    {
                        ModeBateau = Constante.MODE_OCTOPUS_NOM;
                        this.nombreBouletsParShoot = (int)Constante.MODE_OCTOPUS[0];
                        this.angleBoulets = (int)Constante.MODE_OCTOPUS[1];
                        this.TempsRechargementCanon = Constante.MODE_OCTOPUS[2];
                        this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;
                    }
                    else if (classe == Constante.MODE_QUATRO_PLUS_NOM)
                    {
                        ModeBateau = Constante.MODE_QUATRO_PLUS_NOM;
                        this.nombreBouletsParShoot = (int)Constante.MODE_QUATRO_PLUS[0];
                        this.angleBoulets = (int)Constante.MODE_QUATRO_PLUS[1];
                        this.TempsRechargementCanon = Constante.MODE_QUATRO_PLUS[2];
                        this.Degats = (int)((this.Degats / this.nombreBouletsParShoot) * 2 + Constante.MODE_QUATRO_PLUS[3]);
                        this.TailleBoulets = (int)Constante.MODE_QUATRO_PLUS[4];
                    }
                }
                else if (ModeBateau == Constante.MODE_DOUBLE_NOM)
                {
                    if (classe == Constante.MODE_TRIO_NOM)
                    {
                        ModeBateau = Constante.MODE_TRIO_NOM;
                        this.nombreBouletsParShoot = (int)Constante.MODE_TRIO[0];
                        this.angleBoulets = (int)Constante.MODE_TRIO[1];
                        this.TempsRechargementCanon = Constante.MODE_TRIO[2];
                        this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;
                        this.espacementBoulets = (int)Constante.MODE_TRIO[3];
                        this.TailleBoulets = (int)Constante.MODE_TRIO[4];
                    }
                    else if (classe == Constante.MODE_MEGA_NOM)
                    {
                        ModeBateau = Constante.MODE_MEGA_NOM;
                        this.nombreBouletsParShoot = (int)Constante.MODE_MEGA[0];
                        this.angleBoulets = (int)Constante.MODE_MEGA[1];
                        this.TempsRechargementCanon = Constante.MODE_MEGA[2];
                        this.Degats = (int)((this.Degats / this.nombreBouletsParShoot) * 2 + Constante.MODE_MEGA[3]);
                        this.TailleBoulets = (int)Constante.MODE_MEGA[4];
                    }
                }
                else if (ModeBateau == Constante.MODE_POMPE_NOM)
                {
                    if (classe == Constante.MODE_SNIPER_NOM)
                    {
                        ModeBateau = Constante.MODE_SNIPER_NOM;
                        this.nombreBouletsParShoot = (int)Constante.MODE_SNIPER[0];
                        this.angleBoulets = (int)Constante.MODE_SNIPER[1];
                        this.TempsRechargementCanon = Constante.MODE_SNIPER[2];
                        this.Degats = (int)((this.Degats / this.nombreBouletsParShoot) * 2 + Constante.MODE_SNIPER[3]);
                        this.espacementBoulets = (int)Constante.MODE_SNIPER[4];
                    }
                    else if (classe == Constante.MODE_MK30_NOM)
                    {
                        ModeBateau = Constante.MODE_MK30_NOM;
                        this.nombreBouletsParShoot = (int)Constante.MODE_MK30[0];
                        this.angleBoulets = (int)Constante.MODE_MK30[1];
                        this.TempsRechargementCanon = Constante.MODE_MK30[2];
                        this.Degats = (int)((this.Degats / this.nombreBouletsParShoot) * 2 + Constante.MODE_MK30[3]);
                        this.TailleBoulets = (int)Constante.MODE_MK30[4];
                    }
                }
                if (classe == Constante.MODE_EVENTAILLE_NOM)
                {
                    ModeBateau = Constante.MODE_EVENTAILLE_NOM;
                    this.nombreBouletsParShoot = (int)Constante.MODE_EVENTAILLE[0];
                    this.angleBoulets = (int)Constante.MODE_EVENTAILLE[1];
                    this.TempsRechargementCanon = Constante.MODE_EVENTAILLE[2];
                    this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;
                }
                this.NiveauClasse = 2;
            }

            // Masquer le menu de sélection de classe
            fenetre.menuClasse.Visibility = Visibility.Hidden;
        }

        public override void Apparaitre(Point position)
        {
            base.Apparaitre(position);
            DeplaceCameraVers(position);
        }

        public override void PlusDeVie()
        {
            base.PlusDeVie();

            NombreMort++;

            MoteurJeu.Fenetre.gridReapparition.Visibility = Visibility.Visible;
            MinuteurReapparition.Start();
        }

        private void Reapparition(object? sender, EventArgs e)
        {
            MinuteurReapparition.Stop();
            Point nouvellePosition = MoteurJeu.GestionCarte.GenererPositionAleatoire(CanvaElement.Width, CanvaElement.Height);

            Vie = VieMax;
            EstMort = false;

            MoteurJeu.Fenetre.gridReapparition.Visibility = Visibility.Hidden;

            MettreAJour();
            Apparaitre(nouvellePosition);
        }

        public override void Deplacer(double deltaTemps)
        {
            if (EstMort)
                return;

            double ancienX = PositionX;
            double ancienY = PositionY;

            base.Deplacer(deltaTemps);

            if (ancienX != PositionX || ancienY != PositionY)
                DeplaceCameraVers(Position);
        }

        protected override bool PeutAllerVers(double nouvellePosX, double nouvellePosY)
        {
            Collision nouvelleCollision = new Collision(nouvellePosX, nouvellePosY, CanvaElement.Width, CanvaElement.Height, AngleRotation());

            if (nouvelleCollision.CollisionDevantAvec(MoteurJeu.GestionCarte.Ile.CollisionRectangle))
                return false;

            foreach (Obstacle obstacle in MoteurJeu.GestionCarte.Obstacles)
            {
                if (nouvelleCollision.CollisionDevantAvec(obstacle.CollisionRectangle))
                    return false;
            }

            foreach (ObjetRecompense objetBonus in MoteurJeu.ObjetsBonus)
            {
                if (nouvelleCollision.CollisionDevantAvec(objetBonus.CollisionRectangle))
                    return false;
            }

            foreach (Ennemi ennemi in MoteurJeu.GestionVagues.EnnemisActuels)
            {
                if (nouvelleCollision.CollisionDevantAvec(ennemi.CollisionRectangle)) 
                    return false;
            }

            return true;
        }

        private void DeplaceCameraVers(Point position)
        {
            double centreFenetreX = MoteurJeu.Fenetre.ActualWidth / 2;
            double centreFenetreY = MoteurJeu.Fenetre.ActualHeight / 2;

            // Calcul de la nouvelle position de la caméra pour centrer le tireur
            cameraX = position.X - centreFenetreX + CanvaElement.Width / 2;
            cameraY = position.Y - centreFenetreY + CanvaElement.Height / 2;

            // Empêcher la caméra de sortir des limites de la carte
            cameraX = Math.Max(0, Math.Min(cameraX, Carte.Width - MoteurJeu.Fenetre.ActualWidth));
            cameraY = Math.Max(0, Math.Min(cameraY, Carte.Height - MoteurJeu.Fenetre.ActualHeight));

            // Appliquer le déplacement du Canvas (monde)
            Canvas.SetLeft(Carte, -cameraX);
            Canvas.SetTop(Carte, -cameraY);
        }

        public void AjouterExperience(int experience)
        {
            Experience += experience;

            if (Experience >= ExperienceMax)
            {
                NiveauSuivant();

            }

            ActualiserMenuAmelioration();
        }

        private void NiveauSuivant()
        {
            NouveauNiveau = true;
            PointsAmeliorations++;

            Niveau += 1;

            if (Niveau >= 5 && ModeBateau == "classique")
            {
                AfficherMenuClasse();
            }
            else if (Niveau >= 15 && ModeBateau != "classique" ) {
                AfficherMenuClasse();
            }

            Experience = 0;
            ExperienceMax = (int)(ExperienceMax * Constante.MULTIPLICATEUR_NIVEAU);
        }

        private void ActualiserHUD()
        {
            fenetre.barreVieJoueur.Value = Vie;
            fenetre.barreVieJoueur.Maximum = VieMax;
            fenetre.txtVieJoueur.Text = Vie + "/" + VieMax + " PV";

            fenetre.txtNbPiratesCoule.Text = NombreCoule.ToString();

            ActualiserMenuAmelioration();
        }

        private void ActualiserMenuAmelioration()
        {
            fenetre.txtPointAmelio.Text = PointsAmeliorations.ToString();

            fenetre.barreXPAmelio.Maximum = ExperienceMax;
            fenetre.barreXPAmelio.Value = Experience;
            fenetre.txtXPAmelio.Text = Experience + "/" + ExperienceMax + " XP";

            fenetre.txtVieJoueurAmelio.Text = VieMax.ToString();
            fenetre.txtDegatsJoueurAmelio.Text = Degats.ToString();
            fenetre.txtVitesseJoueurAmelio.Text = VitesseMax.ToString();

            fenetre.txtNiveau.Text = Niveau.ToString();

            if (NouveauNiveau && !fenetre.menuActif)
            {
                fenetre.ellipseNouveauNiveau.Visibility = Visibility.Visible;
            }
            else
            {
                fenetre.ellipseNouveauNiveau.Visibility = Visibility.Hidden;
            }
        }

        protected override void QuandBouletTire()
        {
            MoteurJeu.GestionSons.JoueSon();
        }

        public override void MettreAJour()
        {
            base.MettreAJour();
            ActualiserHUD();
        }

        public void AmelioVie()
        {
            if (PointsAmeliorations > 0)
            {
                VieMax += Constante.AMELIO_VIE_MAX;
                PointsAmeliorations--;
                MettreAJour();
            }
        }
        public void AmelioVitesse()
        {
            if (PointsAmeliorations > 0)
            {
                VitesseMax = Math.Round(VitesseMax + Constante.AMELIO_VITESSE, 1);
                PointsAmeliorations--;
                MettreAJour();
            }
        }
        public void AmelioDegats()
        {
            if (PointsAmeliorations > 0)
            {
                Degats += Constante.AMELIO_DEGATS;
                PointsAmeliorations--;
                MettreAJour();
            }
        }
    }
}
