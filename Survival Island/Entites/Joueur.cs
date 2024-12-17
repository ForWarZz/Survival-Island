using Survival_Island.Entites.Navire;
using Survival_Island.Outils;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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

        public string ModeBateau { get; set; }

        public int niveauClasse;

        private int nombreBouletsParShoot;
        private int angleBoulets;
        private int espacementBoulets;

        public BitmapImage[] images;


        public Joueur(Canvas carte, MoteurJeu moteurJeu, BitmapImage bitmapImage) :
            base(carte, moteurJeu, bitmapImage, false, Constante.JOUEUR_VIE_MAX, Constante.JOUEUR_DEGATS, Constante.JOUEUR_VITESSE, Constante.JOUEUR_RECHARGEMENT_CANON)
        {
            fenetre = moteurJeu.Fenetre;
            Niveau = 0;
            PointsAmeliorations = 0;
            Experience = 0;
            ExperienceMax = Constante.JOUEUR_EXPERIENCE_MAX_N1;


            ModeBateau = "classique";
            ModeTriche = false;

            nombreBouletsParShoot = 1;

            espacementBoulets = 0;

            angleBoulets = 0;

            niveauClasse = 0;

            InitImages();
        }

        public void InitImages()
        {
            BitmapImage bateauR = new BitmapImage(new Uri(Chemin.IMAGE_BATEAU_ROUGE));
            BitmapImage bateauB = new BitmapImage(new Uri(Chemin.IMAGE_BATEAU_BLEU));
            BitmapImage bateauJ = new BitmapImage(new Uri(Chemin.IMAGE_BATEAU_JAUNE));
            BitmapImage bateauV = new BitmapImage(new Uri(Chemin.IMAGE_BATEAU_VERT));

            images = [bateauR, bateauB, bateauJ, bateauV];
        }

        public void TirerBoulet()
        {
            double centreBateauX = PositionX + CanvaElement.Width / 2;
            double centreBateauY = PositionY + CanvaElement.Height / 2;

            if (TempsDernierTir > 0)
                return;
            TempsDernierTir = TempsRechargementCanon;

            
            double espaceBoulets = angleBoulets / (nombreBouletsParShoot);

            double angleOrientation = Math.Atan2(Orientation.Y, Orientation.X);

            double espacementBouletsX = 0;
            double espacementBouletsY = 0;

            if (espacementBoulets > 0)
            {
                // Calcul du décalage en X et Y
                espacementBouletsX = -Math.Sin(angleOrientation) * espacementBoulets;
                espacementBouletsY = Math.Cos(angleOrientation) * espacementBoulets;
            }


            // Générer chaque boulet avec une direction légèrement différente
            for (int i = 0; i < nombreBouletsParShoot; i++)
            {
                // Calcul de l'angle pour ce boulet
                double offsetAngle = (i - (nombreBouletsParShoot ) / 2.0) * (espaceBoulets * Math.PI / 180.0);
                double angleBoulet = angleOrientation + offsetAngle;

                // Calcul de la direction du boulet
                Vector directionBoulet = new Vector(Math.Cos(angleBoulet), Math.Sin(angleBoulet));

                // Création et apparition du boulet
                Boulet boulet = new Boulet(Carte, directionBoulet, this);
                boulet.Apparaitre(centreBateauX - boulet.CanvaElement.Width / 2    +i * espacementBouletsX,
                                    centreBateauY - boulet.CanvaElement.Height / 2 +i* espacementBouletsY);

                // Ajouter le boulet au moteur du jeu
                MoteurJeu.Boulets.Add(boulet);
            }
        }

        private void AfficherMenuClasse()
        {
            if (ModeBateau == "classique")
            {
                //Affichage du menu de niveau 5

                fenetre.menuClasse.Visibility = Visibility.Visible;

                fenetre.btnQuatre.Click += (sender, e) => { ChoisirClasse("quatre"); };
                fenetre.btnSpread.Click += (sender, e) => { ChoisirClasse("spread"); };
                fenetre.btnDuo.Click += (sender, e) => { ChoisirClasse("duo"); };
            }
            else if (ModeBateau == "quatre")
            {

                fenetre.menuClasse.Visibility = Visibility.Visible;


                fenetre.btnQuatre.Content = "Octopus";
                fenetre.btnQuatre.Click += (sender, e) => { ChoisirClasse("octopus"); };

                fenetre.btnSpread.Content = "QuatroPlus";
                fenetre.btnSpread.Click += (sender, e) => { ChoisirClasse("quatroPlus"); };
                fenetre.btnDuo.Click += (sender, e) => { ChoisirClasse("duo"); };
            }
        }

        private void ChoisirClasse(string classe)
        {
            // Appliquer les modifications en fonction de la classe choisie
            if (Niveau >= 5 && ModeBateau=="classique")
            {
                if (classe == "quatre")
                {
                    ModeBateau = "quatre";
                    this.nombreBouletsParShoot = 4;
                    this.angleBoulets = 360;
                    this.TempsRechargementCanon = 1.1;
                    this.Degats = 15;
                }
                else if (classe == "duo")
                {
                    ModeBateau = "duo";
                    this.nombreBouletsParShoot = 2;
                    this.angleBoulets = 0;
                    this.TempsRechargementCanon = 1.1;
                    this.Degats = 15;
                    this.espacementBoulets = 10;
                }
                else if (classe == "spread")
                {

                    ModeBateau = "spread";
                    this.nombreBouletsParShoot = 3;
                    this.angleBoulets = 20;
                    this.TempsRechargementCanon = 1.1;
                    this.Degats = 15;

                }
                this.niveauClasse = 1;
            }

            else if (Niveau >= 15) {
                if (ModeBateau == "quatre")
                {
                    if (classe == "octopus")
                    {

                        ModeBateau = "octopus";
                        this.nombreBouletsParShoot = 8;
                        this.angleBoulets = 360;
                        this.TempsRechargementCanon = 1.3;
                        this.Degats = 20;

                    }
                    else if (classe == "quatroPlus")
                    {

                        ModeBateau = "quatroPlus";
                        this.nombreBouletsParShoot = 4;
                        this.angleBoulets = 360;
                        this.TempsRechargementCanon = 0.8;
                        this.Degats = 40;
                        this.taille_boulets = 30;

                    }

                    this.niveauClasse = 2;
                }
            }


            // Masquer le menu de sélection de classe
            fenetre.menuClasse.Visibility = Visibility.Hidden;

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
                VitesseMax = Math.Round(VitesseMax + Constante.AMELIO_VITESSE,1);
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
