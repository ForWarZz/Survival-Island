using Survival_Island.carte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Survival_Island.joueur
{
    internal class Joueur : Collision
    {
        public int vie { get; private set; } = Constante.JOUEUR_VIE_MAX;

        public NavireCaracteristique caracteristique { get; set; }
        public double tempsDernierBoulet { get; set; } = 0;
        public bool canonActif { get; set; }
        public bool deplacement { get; set; }

        public Vector orientation { get; private set; }

        private MainWindow fenetre;

        private ProgressBar progressBar;
        private TextBlock valeurProgressBar;

        private DispatcherTimer rotationTemps;
        private double angleCible;
        private double angleActuel;

        private BitmapImage bitmapBateau;
        private List<Boulet> boulets;

        private double cameraX, cameraY;

        public Joueur(Canvas carte, MainWindow fenetre, List<Boulet> boulets): base(carte, false)
        {
            this.fenetre = fenetre;
            this.boulets = boulets;

            progressBar = fenetre.progressVieNavire;
            valeurProgressBar = fenetre.txtVieNavire;

            InitBitmaps();

            canvaElement = new Image();
            ((Image) canvaElement).Source = bitmapBateau;
            canvaElement.Width = Constante.LARGEUR_NAVIRE;
            canvaElement.Height = Constante.HAUTEUR_NAVIRE;

            caracteristique = new NavireCaracteristique();

            InitRotationTemps();
            UpdateHUD();
        }

        private void InitBitmaps()
        {
            bitmapBateau = new BitmapImage(new Uri(Chemin.IMAGE_BATEAU_ROUGE));
        }

        private void InitRotationTemps()
        {
            rotationTemps = new DispatcherTimer();
            rotationTemps.Interval = TimeSpan.FromMilliseconds(Constante.TEMPS_ROTATION_NAVIRE);
            rotationTemps.Tick += AnimationRotation;
        }

        public void Apparaitre()
        {
            double centreEcranX = (carte.Width - canvaElement.Width) / 2;
            double centreEcranY = (carte.Height - canvaElement.Height) / 2;

            canvaElement.RenderTransform = new RotateTransform(0, canvaElement.Width / 2, canvaElement.Height / 2);

            Apparaitre(centreEcranX, centreEcranY);
            InitCamera();
        }

        public void UpdateOrientation(Point position)
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

        // Animation de la rotation du bateau: TODO - A refaire/améliorer pour plus de fluidité et meilleur UX
        private void AnimationRotation(object? sender, EventArgs e)
        {
            double diffAngle = angleCible - angleActuel;

            if (diffAngle > 180) diffAngle -= 360;
            if (diffAngle < -180) diffAngle += 360;

            if (Math.Abs(diffAngle) <= Constante.TOLERANCE_ANGLE_ROTATION)   // On ajoute une tolérance pour éviter les oscillations infinies
            {
                angleActuel = angleCible;
                rotationTemps.Stop();
            }
            else
            {
                angleActuel += Math.Sign(diffAngle) * 5;
            }

            // On calcule le vetceur directeur (utilisé pour le tir des boulets ou encore le mouvement)
            double angleRad = (angleActuel + 90) * Math.PI / 180;
            orientation = new Vector(Math.Cos(angleRad), Math.Sin(angleRad));
            orientation.Normalize();

            canvaElement.RenderTransform = new RotateTransform(angleActuel, canvaElement.Width / 2, canvaElement.Height / 2);
        }

        public void TirerBoulet()
        {
            double centreBateauX = positionX + canvaElement.Width / 2;
            double centreBateauY = positionY + canvaElement.Height / 2;

            if (tempsDernierBoulet > 0) return;
            tempsDernierBoulet = caracteristique.tempsRechargementCanon;

            // On créer un nouveau boulet
            Boulet boulet = new Boulet(carte, orientation);
            boulet.Apparaitre(centreBateauX - boulet.canvaElement.Width / 2, centreBateauY - boulet.canvaElement.Height / 2);

            boulets.Add(boulet);
        }

        /*
                public void CheckCollisions(List<Ennemi> listeEnnemis)
                {
                    for (int i = 0; i < boulets.Count; i++)
                    {
                        Boulet boulet = boulets[i];
                        double bouletX = Canvas.GetLeft(boulet.boulet);
                        double bouletY = Canvas.GetTop(boulet.boulet);

                        // Vérifier les collisions avec les ennemis
                        foreach (var ennemi in listeEnnemis)
                        {
                            Rect ennemiRect = new Rect(Canvas.GetLeft(ennemi.image), Canvas.GetTop(ennemi.image), ennemi.image.Width, ennemi.image.Height);
                            Rect bouletRect = new Rect(bouletX, bouletY, boulet.boulet.Width, boulet.boulet.Height);

                            if (ennemiRect.IntersectsWith(bouletRect))
                            {
                                ennemi.RecevoirDegats(caracteristique.degats);
                                carte.Children.Remove(boulet.boulet);
                                boulets.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }
                }*/

        /*public void DeplaceBateau(double deltaX, double deltaY)
        {
            double posX = Canvas.GetLeft(canvaElement);
            double posY = Canvas.GetTop(canvaElement);

            posX += deltaX;
            posY += deltaY;

            double maxX = carte.Width - canvaElement.Width;
            double maxY = carte.Height - canvaElement.Height;

            Console.WriteLine("Carte X : " + maxX);
            Console.WriteLine("Carte Y : " + maxY);
            Console.WriteLine("Pos X : " + posX + " Pos Y : " + posY);

            if (posX < maxX && posX > 0)
                Canvas.SetLeft(canvaElement, posX);

            if (posY < maxY && posY > 0) 
                Canvas.SetTop(canvaElement, posY);
        }*/

        public void DeplaceBateau(double deltaX, double deltaY)
        {
            double nouvellePosX = positionX + deltaX;
            double nouvellePosY = positionY + deltaY;

            double maxX = carte.Width - canvaElement.Width;
            double maxY = carte.Height - canvaElement.Height;

            // Empêcher le bateau de sortir des limites de la carte
            positionX = Math.Max(0, Math.Min(nouvellePosX, maxX));
            positionY = Math.Max(0, Math.Min(nouvellePosY, maxY));

            DeplaceCameraVers(positionX, positionY);
        }

        private void DeplaceCameraVers(double positionX, double positionY)
        {
            double centreFenetreX = fenetre.ActualWidth / 2;
            double centreFenetreY = fenetre.ActualHeight / 2;

            // Calcul de la nouvelle position de la caméra pour centrer le bateau
            cameraX = positionX - centreFenetreX + (canvaElement.Width / 2);
            cameraY = positionY - centreFenetreY + (canvaElement.Height / 2);

            // Empêcher la caméra de sortir des limites de la carte
            cameraX = Math.Max(0, Math.Min(cameraX, carte.Width - fenetre.ActualWidth));
            cameraY = Math.Max(0, Math.Min(cameraY, carte.Height - fenetre.ActualHeight));

            // Appliquer le déplacement du Canvas (monde)
            Canvas.SetLeft(carte, -cameraX);
            Canvas.SetTop(carte, -cameraY);
        }

        // Initialisation de la caméra pour centrer le bateau au début
        private void InitCamera()
        {
            cameraX = carte.Width / 2 - fenetre.ActualWidth / 2;
            cameraY = carte.Height / 2 - fenetre.ActualHeight / 2;

            Canvas.SetLeft(carte, -cameraX);
            Canvas.SetTop(carte, -cameraY);
        }

        public void InfligerDegats(int degats)
        {
            vie -= degats;
            UpdateHUD();
        }

        public void UpdateHUD()
        {
            progressBar.Value = vie * 100 / caracteristique.vieMax;
            valeurProgressBar.Text = vie + "/" + caracteristique.vieMax + " PV";
        }
    }
}