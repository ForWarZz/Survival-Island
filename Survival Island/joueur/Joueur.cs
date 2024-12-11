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
using System.Windows.Threading;

namespace Survival_Island.joueur
{
    internal class Joueur
    {
        public Image bateau { get; set; }
        public NavireCaracteristique caracteristique { get; set; }

        private Canvas carte;

        private DispatcherTimer rotationTemps;
        private double angleCible;
        private double angleActuel;

        private BitmapImage bitmapBateau;

        private List<Boulet> boulets = new List<Boulet>();
        private double tempsDernierBoulet = 0;

        public Joueur(Canvas carte)
        {
            this.carte = carte;

            InitBitmaps();

            bateau = new Image();
            bateau.Source = bitmapBateau;
            bateau.Width = 50;
            bateau.Height = 100;

            caracteristique = new NavireCaracteristique();

            InitRotationTemps();
        }

        private void InitBitmaps()
        {
            bitmapBateau = new BitmapImage(new Uri(Chemin.IMAGE_BATEAU_ROUGE));
        }

        private void InitRotationTemps()
        {
            rotationTemps = new DispatcherTimer();
            rotationTemps.Interval = TimeSpan.FromMilliseconds(20);
            rotationTemps.Tick += AnimationRotation;
        }

        public void ApparaitreBateau()
        {
            double centreEcranX = (carte.ActualWidth - bateau.Width) / 2;
            double centreEcranY = (carte.ActualHeight - bateau.Height) / 2;

            Canvas.SetLeft(bateau, centreEcranX);
            Canvas.SetTop(bateau, centreEcranY);

            bateau.RenderTransform = new RotateTransform(0, bateau.Width / 2, bateau.Height / 2);
            carte.Children.Add(bateau);
        }

        public void UpdateOrientation(Point position)
        {
            double bateauX = Canvas.GetLeft(bateau) + bateau.Width / 2;
            double bateauY = Canvas.GetTop(bateau) + bateau.Height / 2;

            double deltaX = position.X - bateauX;
            double deltaY = position.Y - bateauY;

            angleCible = Math.Atan2(deltaY, deltaX) * 180 / Math.PI - 90;

            if (!rotationTemps.IsEnabled)
            {
                rotationTemps.Start();
            }
        }

        public void DeplacerBoulets()
        {
            // On parcourt la liste des boulets à l'envers pour pouvoir supprimer des éléments
            for (int i = 0; i < boulets.Count; i++)
            {
                Boulet boulet = boulets[i];
                double bouletX = Canvas.GetLeft(boulet.image);
                double bouletY = Canvas.GetTop(boulet.image);

                bouletX += boulet.direction.X * 10; // Vitesse du boulet
                bouletY += boulet.direction.Y * 10;

                Canvas.SetLeft(boulet.image, bouletX);
                Canvas.SetTop(boulet.image, bouletY);

                // Supprimer le boulet si hors écran
                if (bouletX < 0 || bouletY < 0 ||
                    bouletX > carte.ActualWidth + boulet.image.Width || bouletY > carte.ActualHeight + boulet.image.Width)
                {
                    carte.Children.Remove(boulet.image);
                    boulets.RemoveAt(i);
                }
            }

            if (tempsDernierBoulet > 0)
            {
                tempsDernierBoulet -= 1.0 / 60.0;   // On suppose 60 FPS 
            }

            CheckCollisions();
        }

        private void CheckCollisions()
        {

        }

        private void AnimationRotation(object? sender, EventArgs e)
        {
            double diffAngle = angleCible - angleActuel;

            if (Math.Abs(diffAngle) <= 3)   // On ajoute une tolérance pour éviter les oscillations infinies
            {
                angleActuel = angleCible;
                rotationTemps.Stop();
            }
            else
            {
                angleActuel += Math.Sign(diffAngle) * 5;
            }

            bateau.RenderTransform = new RotateTransform(angleActuel, bateau.Width / 2, bateau.Height / 2);
        }


        public void TirerBoulet()
        {
            if (tempsDernierBoulet > 0) return;
            tempsDernierBoulet = caracteristique.tempsRechargementCanon;

            // On calcule le vecteur directeur du boulet, par rapport a l'angle de direction du bateau
            double angleRad = (angleActuel + 90) * Math.PI / 180;
            Vector direction = new Vector(Math.Cos(angleRad), Math.Sin(angleRad));

            double centreBateauX = Canvas.GetLeft(bateau) + bateau.Width / 2;
            double centreBateauY = Canvas.GetTop(bateau) + bateau.Height / 2;

            // On créer un nouveau boulet
            Image bouletImage = new Image();
            /*            bouletImage.Source = bitmapBouletCanon;
                        bouletImage.Width = bitmapBouletCanon.Width;
                        bouletImage.Height = bitmapBouletCanon.Height;*/

            Canvas.SetLeft(bouletImage, centreBateauX - bouletImage.Width / 2);
            Canvas.SetTop(bouletImage, centreBateauY - bouletImage.Height / 2);

            carte.Children.Add(bouletImage);

            Boulet boulet = new Boulet(bouletImage, direction);
            boulets.Add(boulet);
        }
    }
}