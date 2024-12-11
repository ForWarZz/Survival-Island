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
    internal class Joueur
    {
        public Image bateau { get; set; }
        public NavireCaracteristique caracteristique { get; set; }

        public int vie {  get; set; }
        public int vieMax { get; set; }
        private Canvas carte;

        private DispatcherTimer rotationTemps;
        private double angleCible;
        private double angleActuel;

        private BitmapImage bitmapBateau;

        private List<Boulet> boulets = new List<Boulet>();
        private double tempsDernierBoulet = 0;
        private int vieDeBase = 100;

        public Joueur(Canvas carte)
        {
            this.carte = carte;

            this.vie = vieDeBase;
            this.vieMax = vieDeBase;
            InitBitmaps();

            bateau = new Image();
            bateau.Source = bitmapBateau;
            bateau.Width = Constante.LARGEUR_NAVIRE;
            bateau.Height = Constante.HAUTEUR_NAVIRE;

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
            rotationTemps.Interval = TimeSpan.FromMilliseconds(Constante.TEMPS_ROTATION_NAVIRE);
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
                double bouletX = Canvas.GetLeft(boulet.boulet);
                double bouletY = Canvas.GetTop(boulet.boulet);

                bouletX += boulet.direction.X * Constante.VITESSE_BOULET; // Vitesse du boulet
                bouletY += boulet.direction.Y * Constante.VITESSE_BOULET;

                Canvas.SetLeft(boulet.boulet, bouletX);
                Canvas.SetTop(boulet.boulet, bouletY);

                // Supprimer le boulet si hors écran
                if (bouletX < 0 || bouletY < 0 ||
                    bouletX > carte.ActualWidth + boulet.boulet.Width || bouletY > carte.ActualHeight + boulet.boulet.Width)
                {
                    carte.Children.Remove(boulet.boulet);
                    boulets.RemoveAt(i);
                }
            }

            if (tempsDernierBoulet > 0)
            {
                tempsDernierBoulet -= 1.0 / 60.0;   // On suppose 60 FPS, pour convertir
            }

        }


        private void AnimationRotation(object? sender, EventArgs e)
        {
            double diffAngle = angleCible - angleActuel;

            if (diffAngle > 180) diffAngle -= 360;
            if (diffAngle < -180) diffAngle += 360;

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
            Ellipse bouletImage = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Black
            };

            Canvas.SetLeft(bouletImage, centreBateauX - bouletImage.Width / 2);
            Canvas.SetTop(bouletImage, centreBateauY - bouletImage.Height / 2);

            carte.Children.Add(bouletImage);

            Boulet boulet = new Boulet(bouletImage, direction);
            boulets.Add(boulet);
        }
        

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
        }

    }
}