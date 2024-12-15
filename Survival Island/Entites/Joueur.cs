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
            double centreFenetreX = MoteurJeu.Fenetre.ActualWidth / 2;
            double centreFenetreY = MoteurJeu.Fenetre.ActualHeight / 2;

            // Calcul de la nouvelle position de la caméra pour centrer le tireur
            cameraX = positionX - centreFenetreX + CanvaElement.Width / 2;
            cameraY = positionY - centreFenetreY + CanvaElement.Height / 2;

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
            PointsAmeliorations++;

            Niveau += 1;
            if (Niveau == 5)
            {
                ModeBateau = new ModeDouble(Carte, MoteurJeu.Boulets, this);
            }

            Experience = 0;
            ExperienceMax = (int)(ExperienceMax * Constante.MULTIPLICATEUR_NIVEAU);
        }

        private void ActualiserHUD()
        {
            fenetre.barreVieJoueur.Value = Vie;
            fenetre.barreVieJoueur.Maximum = VieMax;
            fenetre.txtVieJoueur.Text = Vie + "/" + VieMax + " PV";
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
                VitesseMax += Constante.AMELIO_VITESSE;
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
