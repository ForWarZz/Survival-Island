﻿using Survival_Island.Entites.Base;
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

        private DispatcherTimer minuteurReapparition;

        public Joueur(MoteurJeu moteurJeu) :
            base(moteurJeu, moteurJeu.GestionImages.BateauxJoueur[0], Brushes.Black, 
                false, Constante.JOUEUR_VIE_MAX, Constante.JOUEUR_DEGATS, Constante.JOUEUR_VITESSE, 
                Constante.JOUEUR_RECHARGEMENT_CANON)
        {
            fenetre = moteurJeu.Fenetre;
            Niveau = 0;
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
            minuteurReapparition = new DispatcherTimer();
            minuteurReapparition.Interval = Constante.TEMPS_REAPPARITION;
            minuteurReapparition.Tick += Reapparition;
        }

        private void AfficherMenuClasse()
        {
            if (ModeBateau == "classique")
            {
                //Affichage du menu de niveau 5

                fenetre.menuClasse.Visibility = Visibility.Visible;

                fenetre.btnQuatre.Click += (sender, e) => { ChoisirClasse("quatre"); };
                fenetre.btnPompe.Click += (sender, e) => { ChoisirClasse("pompe"); };
                fenetre.btnDouble.Click += (sender, e) => { ChoisirClasse("double"); };
            }
            else if (ModeBateau == "quatre")
            {

                fenetre.menuClasse.Visibility = Visibility.Visible;


                fenetre.btnQuatre.Content = "Octopus";
                fenetre.btnQuatre.Click += (sender, e) => { ChoisirClasse("octopus"); };

                fenetre.btnPompe.Content = "QuatroPlus";
                fenetre.btnPompe.Click += (sender, e) => { ChoisirClasse("quatroPlus"); };

                fenetre.btnDouble.Content = "Eventaille";
                fenetre.btnDouble.Click += (sender, e) => { ChoisirClasse("Eventaille"); };
            }
            else if (ModeBateau == "double")
            {

                fenetre.menuClasse.Visibility = Visibility.Visible;


                fenetre.btnQuatre.Content = "trio";
                fenetre.btnQuatre.Click += (sender, e) => { ChoisirClasse("trio"); };

                fenetre.btnPompe.Content = "MegaDouble";
                fenetre.btnPompe.Click += (sender, e) => { ChoisirClasse("MegaDouble"); };

                fenetre.btnDouble.Content = "Eventaille";
                fenetre.btnDouble.Click += (sender, e) => { ChoisirClasse("Eventaille"); };
            }
            else if (ModeBateau == "pompe")
            {

                fenetre.menuClasse.Visibility = Visibility.Visible;


                fenetre.btnQuatre.Content = "sniper";
                fenetre.btnQuatre.Click += (sender, e) => { ChoisirClasse("sniper"); };

                fenetre.btnPompe.Content = "mk30";
                fenetre.btnPompe.Click += (sender, e) => { ChoisirClasse("mk30"); };

                fenetre.btnDouble.Content = "Eventaille";
                fenetre.btnDouble.Click += (sender, e) => { ChoisirClasse("Eventaille"); };
            }

            fenetre.btnIgnore.Click += (sender, e) => { ChoisirClasse("ignore"); };
        }

        private void ChoisirClasse(string classe)
        {
            // Appliquer les modifications en fonction de la classe choisie
            if (Niveau >= 5 && ModeBateau=="classique" && classe!="ignore")
            {
                if (classe == "quatre")
                {
                    ModeBateau = "quatre";
                    this.nombreBouletsParShoot = 4;
                    this.angleBoulets = 360;
                    this.TempsRechargementCanon = 1.1;
                    this.Degats = (this.Degats/this.nombreBouletsParShoot)*2;
                }
                else if (classe == "double")
                {
                    ModeBateau = "double";
                    this.nombreBouletsParShoot = 2;
                    this.angleBoulets = 0;
                    this.TempsRechargementCanon = 1.1;
                    this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;
                    this.espacementBoulets = 10;
                }
                else if (classe == "pompe")
                {
                    ModeBateau = "pompe";
                    this.nombreBouletsParShoot = 3;
                    this.angleBoulets = 20;
                    this.TempsRechargementCanon = 1.1;
                    this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;

                }
                this.NiveauClasse = 1;
            }

            else if (Niveau >= 15 && classe != "ignore") {
                if (ModeBateau == "quatre")
                {
                    if (classe == "octopus")
                    {

                        ModeBateau = "octopus";
                        this.nombreBouletsParShoot = 8;
                        this.angleBoulets = 360;
                        this.TempsRechargementCanon = 1.3;
                        this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;

                    }
                    else if (classe == "quatroPlus")
                    {

                        ModeBateau = "quatroPlus";
                        this.nombreBouletsParShoot = 4;
                        this.angleBoulets = 360;
                        this.TempsRechargementCanon = 0.8;
                        this.Degats = ((this.Degats / this.nombreBouletsParShoot) * 2) + 10;
                        this.TailleBoulets = 30;

                    }
                    

                    this.NiveauClasse = 2;
                }

                else if (ModeBateau == "double")
                {
                    if (classe == "trio")
                    {

                        ModeBateau = "trio";
                        this.nombreBouletsParShoot = 3;
                        this.angleBoulets = 0;
                        this.TempsRechargementCanon = 1.0;
                        this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;
                        this.espacementBoulets = 10;
                    }
                    else if (classe == "MegaDouble")
                    {

                        ModeBateau = "MegaDouble";
                        this.nombreBouletsParShoot = 2;
                        this.angleBoulets = 0;
                        this.TempsRechargementCanon = 1.8;
                        this.Degats = ((this.Degats / this.nombreBouletsParShoot) * 2) + 20;
                        this.TailleBoulets = 40;


                    }
                    else if (classe == "Eventaille")
                    {
                        ModeBateau = "Eventaille";
                        this.nombreBouletsParShoot = 6;
                        this.angleBoulets = 180;
                        this.TempsRechargementCanon = 1;
                        this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;
                    }

                    this.NiveauClasse = 2;
                }

                else if (ModeBateau == "pompe")
                {
                    if (classe == "sniper")
                    {

                        ModeBateau = "sniper";
                        this.nombreBouletsParShoot = 12;
                        this.angleBoulets = 5;
                        this.TempsRechargementCanon = 3.0;
                        this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;
                        this.espacementBoulets = 10;
                    }
                    else if (classe == "mk30")
                    {

                        ModeBateau = "mk30";
                        this.nombreBouletsParShoot = 5;
                        this.angleBoulets = 35;
                        this.TempsRechargementCanon = 0.8;
                        this.Degats = ((this.Degats / this.nombreBouletsParShoot) * 2) + 20;
                        this.TailleBoulets = 15;


                    }
                    else if (classe == "Eventaille")
                    {
                        ModeBateau = "Eventaille";
                        this.nombreBouletsParShoot = 6;
                        this.angleBoulets = 180;
                        this.TempsRechargementCanon = 1;
                        this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;
                    }

                    this.NiveauClasse = 2;
                }

                if (classe == "Eventaille")
                {
                    ModeBateau = "Eventaille";
                    this.nombreBouletsParShoot = 6;
                    this.angleBoulets = 160;
                    this.TempsRechargementCanon = 1;
                    this.Degats = (this.Degats / this.nombreBouletsParShoot) * 2;
                }
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
            minuteurReapparition.Start();
        }

        private void Reapparition(object? sender, EventArgs e)
        {
            minuteurReapparition.Stop();
            Point nouvellePosition = MoteurJeu.GestionCarte.GenererPositionAleatoire(CanvaElement.Width, CanvaElement.Height);

            Vie = VieMax;
            EstMort = false;

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
                if (nouvelleCollision.EnCollisionAvec(ennemi.CollisionRectangle)) 
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

            fenetre.txtNombreTue.Text = "Ennemis tués : " + NombreCoule;

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

        protected override void BouletTire()
        {
            MoteurJeu.GestionSons.SoundPlayerTire.Play();
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
