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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Survival_Island.joueur
{
    internal class Joueur: Collision
    {
        public int vie { get; set; } = Constante.JOUEUR_VIE_MAX;

        public NavireCaracteristique caracteristique { get; set; }
        public double tempsDernierBoulet = 0;

        public bool canonActif, deplacement;
        public Vector orientation;

        private ProgressBar progressBar;
        private TextBlock valeurProgressBar;

        private DispatcherTimer rotationTemps;
        private double angleCible;
        private double angleActuel;

        private BitmapImage bitmapBateau;
        private List<Boulet> boulets;

        public Joueur(Canvas carte, ProgressBar progressBar, TextBlock valeurProgressBar, List<Boulet> boulets): base(carte)
        {
            this.carte = carte;
            this.progressBar = progressBar;
            this.valeurProgressBar = valeurProgressBar;
            this.boulets = boulets;

            InitBitmaps();

            element = new Image();
            ((Image) element).Source = bitmapBateau;
            element.Width = Constante.LARGEUR_NAVIRE;
            element.Height = Constante.HAUTEUR_NAVIRE;

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

        public override void Apparaitre()
        {
            double centreEcranX = (carte.ActualWidth - element.Width) / 2;
            double centreEcranY = (carte.ActualHeight - element.Height) / 2;

            Canvas.SetLeft(element, centreEcranX);
            Canvas.SetTop(element, centreEcranY);

            element.RenderTransform = new RotateTransform(0, element.Width / 2, element.Height / 2);
            carte.Children.Add(element);
        }

        public void UpdateOrientation(Point position)
        {
            double centreBateauX = Canvas.GetLeft(element) + element.Width / 2;
            double centreBateauY = Canvas.GetTop(element) + element.Height / 2;

            double deltaX = position.X - centreBateauX;
            double deltaY = position.Y - centreBateauY;

            angleCible = Math.Atan2(deltaY, deltaX) * 180 / Math.PI - 90;

            if (!rotationTemps.IsEnabled)
            {
                rotationTemps.Start();
            }
        }

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

            element.RenderTransform = new RotateTransform(angleActuel, element.Width / 2, element.Height / 2);
        }

        public void TirerBoulet()
        {
            double centreBateauX = Canvas.GetLeft(element) + element.Width / 2;
            double centreBateauY = Canvas.GetTop(element) + element.Height / 2;

            if (tempsDernierBoulet > 0) return;
            tempsDernierBoulet = caracteristique.tempsRechargementCanon;

            // On créer un nouveau boulet
            Boulet boulet = new Boulet(carte, orientation);
            boulet.Apparaitre(centreBateauX - boulet.element.Width / 2, centreBateauY - boulet.element.Height / 2);

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