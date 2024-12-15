using Survival_Island.Entites.Navire;
using Survival_Island.Outils;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

        public ModeBateau ModeBateau { get; set; }
        public bool ModeTriche { get; set; }

        public Joueur(Canvas carte, MoteurJeu moteurJeu, BitmapImage bitmapImage) :
            base(carte, moteurJeu, bitmapImage, false, Constante.JOUEUR_VIE_MAX, Constante.JOUEUR_DEGATS, Constante.JOUEUR_VITESSE, Constante.JOUEUR_RECHARGEMENT_CANON)
        {
            fenetre = moteurJeu.Fenetre;
            Niveau = 0;
            PointsAmeliorations = 0;
            Experience = 0;
            ExperienceMax = Constante.JOUEUR_EXPERIENCE_MAX_N1;

            ModeBateau = new ModeClassic(carte, moteurJeu.Boulets, this);
            ModeTriche = false;

            ActualiserHUD();
            ActualiserMenuAmelioration();
        }

        public override void Apparaitre(double posX, double posY)
        {
            base.Apparaitre(posX, posY);
            DeplaceCameraVers(posX, posY);
        }

        public override void Deplacer()
        {
            double ancienX = PositionX;
            double ancienY = PositionY;

            base.Deplacer();

            if (ancienX != PositionX || ancienY != PositionY)
                DeplaceCameraVers(PositionX, PositionY);
        }

        private void DeplaceCameraVers(double positionX, double positionY)
        {
            double centreFenetreX = moteurJeu.Fenetre.ActualWidth / 2;
            double centreFenetreY = moteurJeu.Fenetre.ActualHeight / 2;

            // Calcul de la nouvelle position de la caméra pour centrer le tireur
            cameraX = positionX - centreFenetreX + CanvaElement.Width / 2;
            cameraY = positionY - centreFenetreY + CanvaElement.Height / 2;

            // Empêcher la caméra de sortir des limites de la carte
            cameraX = Math.Max(0, Math.Min(cameraX, carte.Width - moteurJeu.Fenetre.ActualWidth));
            cameraY = Math.Max(0, Math.Min(cameraY, carte.Height - moteurJeu.Fenetre.ActualHeight));

            // Appliquer le déplacement du Canvas (monde)
            Canvas.SetLeft(carte, -cameraX);
            Canvas.SetTop(carte, -cameraY);
        }

        public override void TirerBoulet()
        {
            double centreBateauX = PositionX + CanvaElement.Width / 2;
            double centreBateauY = PositionY + CanvaElement.Height / 2;

            if (tempsDernierTir > 0)
                return;
            tempsDernierTir = tempsRechargementCanon;

            ModeBateau.Tirer(centreBateauX, centreBateauY);
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
            this.Experience += experience;

            if (this.Experience >= ExperienceMax)
            {
                NiveauSuivant();
            }

            ActualiserMenuAmelioration();
        }

        private void NiveauSuivant()
        {
            PointsAmeliorations++;

            Niveau += 1;
            if (Niveau == 5)
            {
                ModeBateau = new ModeDouble(carte, moteurJeu.Boulets, this);
            }

            Experience = 0;
            ExperienceMax = (int)(ExperienceMax * Constante.MULTIPLICATEUR_NIVEAU);
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
            fenetre.txtPointAmelio.Text = PointsAmeliorations.ToString();

            fenetre.barreXPAmelio.Maximum = ExperienceMax;
            fenetre.barreXPAmelio.Value = Experience;
            fenetre.txtXPAmelio.Text = Experience + "/" + ExperienceMax + " XP";

            fenetre.txtVieJoueurAmelio.Text = vieMax.ToString();
            fenetre.txtDegatsJoueurAmelio.Text = degats.ToString();
            fenetre.txtVitesseJoueurAmelio.Text = vitesseMax.ToString();
        }

        public void AmelioVie()
        {
            if (PointsAmeliorations > 0)
            {
                vieMax += Constante.AMELIO_VIE_MAX;
                PointsAmeliorations--;
                ActualiserHUD();
            }
        }
        public void AmelioVitesse()
        {
            if (PointsAmeliorations > 0)
            {
                vitesseMax += Constante.AMELIO_VITESSE;
                PointsAmeliorations--;
                ActualiserHUD();
            }
        }
        public void AmelioDegats()
        {
            if (PointsAmeliorations > 0)
            {
                degats += Constante.AMELIO_DEGATS;
                PointsAmeliorations--;
                ActualiserHUD();
            }
        }
    }
}
