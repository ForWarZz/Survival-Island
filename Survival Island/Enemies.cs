using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Survival_Island
{
    internal class Ennemi
    {
       /* public Image image { get; set; }
        public Vector position { get; set; }
        public double vie { get; set; } = 50;
        public Canvas carte { get; set; }

        private DispatcherTimer minuterieTir;
        private Joueur joueur;
        private List<Boulet> projectiles;
        private double tempsDernierTir;

        public Ennemi(Canvas carte, Joueur joueur, Vector position)
        {
            this.carte = carte;
            this.joueur = joueur;
            this.position = position;
            this.projectiles = new List<Boulet>();

            Initialiserimage();
            InitialiserMinuterieTir();
        }

        private void Initialiserimage()
        {
            image = new Image
            {
                Source = new BitmapImage(new Uri(Chemin.IMAGE_BATEAU_VERT)),
                Width = 50,
                Height = 100
            };

            Canvas.SetLeft(image, position.X);
            Canvas.SetTop(image, position.Y);
            carte.Children.Add(image);
        }

        private void InitialiserMinuterieTir()
        {
            minuterieTir = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2) // Tir toutes les 2 secondes
            };
            minuterieTir.Tick += Tirer;
            minuterieTir.Start();
        }

        public void MettreAJour()
        {
            DeplacerProjectiles();
            VerifierCollisionsAvecJoueur();
        }

        private void Tirer(object? sender, EventArgs e)
        {
            // Calculer la direction vers le joueur
            double joueurX = Canvas.GetLeft(joueur.bateau) + joueur.bateau.Width / 2;
            double joueurY = Canvas.GetTop(joueur.bateau) + joueur.bateau.Height / 2;

            double ennemiX = Canvas.GetLeft(image) + image.Width / 2;
            double ennemiY = Canvas.GetTop(image) + image.Height / 2;

            Vector direction = new Vector(joueurX - ennemiX, joueurY - ennemiY);
            direction.Normalize();

            // Créer le projectile
            Ellipse imageProjectile = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill= Brushes.Red
            };

            Canvas.SetLeft(imageProjectile, ennemiX - imageProjectile.Width / 2);
            Canvas.SetTop(imageProjectile, ennemiY - imageProjectile.Height / 2);
            carte.Children.Add(imageProjectile);

            projectiles.Add(new Boulet(imageProjectile, direction));
        }

        private void DeplacerProjectiles()
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                Boulet boulet = projectiles[i];

                double bouletX = Canvas.GetLeft(boulet.boulet);
                double bouletY = Canvas.GetTop(boulet.boulet);

                bouletX += boulet.direction.X * 8; // Vitesse des projectiles
                bouletY += boulet.direction.Y * 8;

                Canvas.SetLeft(boulet.boulet, bouletX);
                Canvas.SetTop(boulet.boulet, bouletY);

                // Supprimer les projectiles hors écran
                if (bouletX < 0 || bouletY < 0 || bouletX > carte.Width || bouletY > carte.Height)
                {
                    carte.Children.Remove(boulet.boulet);
                    projectiles.RemoveAt(i);
                    i--;
                }
            }
        }

        private void VerifierCollisionsAvecJoueur()
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                Boulet boulet = projectiles[i];

                Rect joueurRect = new Rect(Canvas.GetLeft(joueur.bateau), Canvas.GetTop(joueur.bateau), joueur.bateau.Width, joueur.bateau.Height);
                Rect bouletRect = new Rect(Canvas.GetLeft(boulet.boulet), Canvas.GetTop(boulet.boulet), boulet.boulet.Width, boulet.boulet.Height);

                if (joueurRect.IntersectsWith(bouletRect))
                {
                    joueur.vie -= 10; // Infliger des dégâts au joueur

                    carte.Children.Remove(boulet.boulet);
                    projectiles.RemoveAt(i);
                    i--;

                    // Vérifier si le joueur est mort
                    if (joueur.vie <= 0)
                    {
                        MessageBox.Show("Le joueur est mort !", "Game Over", MessageBoxButton.OK);
                        Application.Current.Shutdown();
                    }
                }
            }
        }

        public void RecevoirDegats(double degats)
        {
            vie -= degats;

            if (vie <= 0)
            {
                carte.Children.Remove(image);
                minuterieTir.Stop();
            }
        }*/
    }
}