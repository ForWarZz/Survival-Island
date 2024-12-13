using Survival_Island.Outils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Survival_Island.Joueurs
{
    public class Joueur : Bateau
    {
        private MainWindow fenetre;
        private double cameraX, cameraY;

        public int niveau { get; private set; }
        public int experience { get; private set; }
        public int experienceMax { get; private set; }

        public int pointsAmeliorations { get; set; }

        public string modeBateu { get; private set; }

        public Joueur(Canvas carte, MoteurJeu moteurJeu, BitmapImage bitmapImage) : 
            base(carte, moteurJeu, bitmapImage, false, Constante.JOUEUR_VIE_MAX, Constante.JOUEUR_DEGATS, Constante.JOUEUR_VITESSE, Constante.JOUEUR_RECHARGEMENT_CANON)
        {
            fenetre = moteurJeu.fenetre;
            niveau = 0;
            pointsAmeliorations = 0;
            experience = 0;
            experienceMax = Constante.JOUEUR_EXPERIENCE_MAX_N1;

            modeBateu = "classic";

            ActualiserHUD();
            ActualiserMenuAmelioration();
        }

        public override void Apparaitre(double posX, double posY)
        {
            base.Apparaitre(posX, posY);
            DeplaceCameraVers(posX, posY);
        }

        public override void ChangerOrientation(Point position)
        {
            double centreBateauX = positionX + canvaElement.Width / 2;
            double centreBateauY = positionY + canvaElement.Height / 2;

            double deltaX = position.X - centreBateauX;
            double deltaY = position.Y - centreBateauY;

            angleCible = Math.Atan2(deltaY, deltaX) * 180 / Math.PI - 90;

            if (!rotationTemps.IsEnabled)
            {
                rotationTemps.Start();
            }
        }

        public override void Deplacer(double deltaX, double deltaY)
        {
            double nouvellePosX = positionX + deltaX;
            double nouvellePosY = positionY + deltaY;

            double maxX = carte.Width - canvaElement.Width;
            double maxY = carte.Height - canvaElement.Height;

            if (!PeutAllerVers(nouvellePosX, nouvellePosY))
                return;

            // Empêcher le bateau de sortir des limites de la carte
            positionX = Math.Max(0, Math.Min(nouvellePosX, maxX));
            positionY = Math.Max(0, Math.Min(nouvellePosY, maxY));

            DeplaceCameraVers(positionX, positionY);
        }

        private void DeplaceCameraVers(double positionX, double positionY)
        {
            double centreFenetreX = moteurJeu.fenetre.ActualWidth / 2;
            double centreFenetreY = moteurJeu.fenetre.ActualHeight / 2;

            // Calcul de la nouvelle position de la caméra pour centrer le bateau
            cameraX = positionX - centreFenetreX + canvaElement.Width / 2;
            cameraY = positionY - centreFenetreY + canvaElement.Height / 2;

            // Empêcher la caméra de sortir des limites de la carte
            cameraX = Math.Max(0, Math.Min(cameraX, carte.Width - moteurJeu.fenetre.ActualWidth));
            cameraY = Math.Max(0, Math.Min(cameraY, carte.Height - moteurJeu.fenetre.ActualHeight));

            // Appliquer le déplacement du Canvas (monde)
            Canvas.SetLeft(carte, -cameraX);
            Canvas.SetTop(carte, -cameraY);
        }

        public override void TirerBoulet()
        {
            double centreBateauX = positionX + canvaElement.Width / 2;
            double centreBateauY = positionY + canvaElement.Height / 2;

            if (tempsDernierTir > 0)
                return;
            tempsDernierTir = tempsRechargementCanon;
            if (modeBateu == "classic")
            {
                // On créer un nouveau boulet
                Boulet boulet = new Boulet(carte, orientation, this);
                boulet.Apparaitre(centreBateauX - boulet.canvaElement.Width / 2, centreBateauY - boulet.canvaElement.Height / 2);

                moteurJeu.boulets.Add(boulet);
            }
            else if (
                modeBateu == "double")
            {

                // On créer un nouveau boulet
                Boulet boulet = new Boulet(carte, orientation, this);
                boulet.Apparaitre(centreBateauX - boulet.canvaElement.Width / 2-8, centreBateauY - boulet.canvaElement.Height / 2);
                moteurJeu.boulets.Add(boulet);

                Boulet boulet1 = new Boulet(carte, orientation, this);
                boulet1.Apparaitre(centreBateauX - boulet.canvaElement.Width / 2 + 8, centreBateauY - boulet.canvaElement.Height / 2);
                moteurJeu.boulets.Add(boulet1);

                Boulet boulet2 = new Boulet(carte, orientation, this);
                boulet2.Apparaitre(centreBateauX - boulet.canvaElement.Width / 2 , centreBateauY - boulet.canvaElement.Height / 2  -8);
                moteurJeu.boulets.Add(boulet2);

                Boulet boulet3 = new Boulet(carte, orientation, this);
                boulet3.Apparaitre(centreBateauX - boulet.canvaElement.Width / 2 , centreBateauY - boulet.canvaElement.Height / 2 +8);
                moteurJeu.boulets.Add(boulet3);

            }
        }

        public override bool InfligerDegats(int degats)
        {
            bool detruit = base.InfligerDegats(degats);
            ActualiserHUD();

            return detruit;
        }

        public override void AjouterVie(int vie)
        {
            base.AjouterVie(vie);
            ActualiserHUD();
        }

        public void AjouterExperience(int experience)
        {
            this.experience += experience;

            if (this.experience >= experienceMax)
            {
                NiveauSuivant();
            }

            ActualiserMenuAmelioration();
        }

        private void NiveauSuivant()
        {
            pointsAmeliorations++;

            niveau += 1;
            if (niveau == 5)
            {
                MessageBox.Show("Chose a class");
                modeBateu = "double";
            }

            experience = 0;
            experienceMax = (int)(experienceMax * Constante.MULTIPLICATEUR_NIVEAU);
        }

        public void ActualiserHUD()
        {
            fenetre.barreVieJoueur.Value = vie;
            fenetre.barreVieJoueur.Maximum = vieMax;
            fenetre.txtVieJoueur.Text = vie + "/" + vieMax + " PV";
            ActualiserMenuAmelioration();
        }

        public void ActualiserMenuAmelioration()
        {
            fenetre.txtPointAmelio.Text = pointsAmeliorations.ToString();
            
            fenetre.barreXPAmelio.Maximum = experienceMax;
            fenetre.barreXPAmelio.Value = experience;
            fenetre.txtXPAmelio.Text = experience + "/" + experienceMax + " XP";

            fenetre.txtVieJoueurAmelio.Text = vieMax.ToString();
            fenetre.txtDegatsJoueurAmelio.Text = degats.ToString();
            fenetre.txtVitesseJoueurAmelio.Text = vitesse.ToString();
        }

        public void AmelioVie()
        {
            if (pointsAmeliorations > 0)
            {
                vieMax += Constante.AMELIO_VIE_MAX;
                pointsAmeliorations--;
                ActualiserHUD();
            }
        }
        public void AmelioVitesse()
        {
            if (pointsAmeliorations > 0)
            {
                vitesse += Constante.AMELIO_VITESSE;
                pointsAmeliorations--;
                ActualiserHUD();
            }
        }
        public void AmelioDegats()
        {
            if (pointsAmeliorations > 0)
            {
                degats += Constante.AMELIO_DEGATS;
                pointsAmeliorations--;
                ActualiserHUD();
            }
        }
    }
}
