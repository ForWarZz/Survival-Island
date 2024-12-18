using Survival_Island.Entites;
using Survival_Island.Outils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Survival_Island
{
    public class GestionVagues
    {
        private MoteurJeu moteurJeu;
        private Ile ile;

        private Canvas carte;
        private BitmapImage bitmapEnnemi;

        public List<Ennemi> EnnemisActuels { get; set; }

        private DispatcherTimer minuteurVague;
        public int Secondes { get; set; }

        private int numeroVague;
        private bool vagueEnCours;

        public GestionVagues(Canvas carte, MoteurJeu moteurJeu, BitmapImage bitmapEnnemi)
        {
            this.moteurJeu = moteurJeu;
            this.carte = carte;
            this.bitmapEnnemi = bitmapEnnemi;

            ile = moteurJeu.Ile;

            EnnemisActuels = new List<Ennemi>();
            Secondes = Constante.TEMPS_ENTRE_VAGUE;

            numeroVague = 0;
            vagueEnCours = false;

            InitMinuteur();
        }

        public void LancerMinuteurVague()
        {
            minuteurVague.Start();
            moteurJeu.Fenetre.txtStatusVague.Text = "Allez chercher des trésors, vous avez " + Secondes + "s !";
            moteurJeu.Fenetre.txtStatusVague.Visibility = Visibility.Visible;
        }

        public void StopperMinuteurVague()
        {
            moteurJeu.Fenetre.txtStatusVague.Visibility = Visibility.Hidden;
        }

        private void InitMinuteur()
        {
            minuteurVague = new DispatcherTimer();
            minuteurVague.Interval = TimeSpan.FromSeconds(1);
            minuteurVague.Tick += LancerVague;
        }

        private void LancerVague(object? sender, EventArgs e)
        {
            Secondes--;
            moteurJeu.Fenetre.txtStatusVague.Text = "Allez chercher des trésors, vous avez " + Secondes + "s !";

            if (Secondes <= 0)
            {
                moteurJeu.Fenetre.txtStatusVague.Text = "Les ennemis arrivent. Défendez votre île !";

                numeroVague++;
                moteurJeu.Fenetre.txtVagueActuelle.Text = "Vague actuelle : " + numeroVague;

                AjouterVague();

                minuteurVague.Stop();
                vagueEnCours = true;

                minuteurVague.Stop();
                Secondes = Constante.TEMPS_ENTRE_VAGUE;
            }
        }

        private void AjouterVague()
        {
            // Nombre d'ennemis basé sur le numéro de vague (par ex : vague 1 = 3 ennemis, vague 2 = 5, etc.)
            int nombreEnnemis = Math.Min(1 + numeroVague, 8);

            int vieMax = 100 + (numeroVague * 10);
            int degats = 10 + numeroVague;
            double tempsRechargement = Math.Max(1.0, 3.0 - (numeroVague * 0.1));


            AjouterEnnemis(nombreEnnemis, vieMax, degats, Constante.JOUEUR_VITESSE, tempsRechargement);
            Console.WriteLine($"Vague {numeroVague} : {nombreEnnemis} ennemis ajoutés !");
        }

        public void MettreAJour()
        {
            if (!vagueEnCours) return;
            if (EnnemisActuels.Count == 0)
            {
                Console.WriteLine($"Vague {numeroVague} terminée !");
                vagueEnCours = false;

                minuteurVague.Start();
            }
        }

        public void AjouterEnnemis(int nombreEnnemis, int vie, int degats, int vitesse, double rechargementCanon)
        {
            for (int i = 0; i < nombreEnnemis; i++)
            {
                Ennemi ennemi = new Ennemi(
                        carte,
                        moteurJeu,
                        bitmapEnnemi,
                        vie,
                        degats,
                        vitesse,
                        rechargementCanon
                );

                Point position = moteurJeu.PositionAleatoireValide(ennemi.CanvaElement.Width, ennemi.CanvaElement.Height, 0, Constante.MARGE_APPARITION_ENNEMI);

                EnnemisActuels.Add(ennemi);
                ennemi.Apparaitre(position);

                Point cible = CalculPointCibleIle(ennemi.Centre);

                ennemi.DefinirCible(cible, moteurJeu.Ile.Centre);
                ennemi.Deplacement = true;
            }
        }

        private Point CalculPointCibleIle(Point ennemi)
        {
            double dx = ile.Centre.X - ennemi.X;
            double dy = ile.Centre.Y - ennemi.Y;

            Vector direction = new Vector(dx, dy);
            direction.Normalize();

            double rayon = ile.CercleEnnemi.Width / 2;

            double xIntersection = ile.Centre.X - direction.X * rayon;
            double yIntersection = ile.Centre.Y - direction.Y * rayon;

            return new Point(xIntersection, yIntersection);
        }
    }
}
