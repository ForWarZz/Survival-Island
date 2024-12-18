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
        private GestionCarte gestionCarte;
        private Ile ile;

        private Canvas carte;

        public List<Ennemi> EnnemisActuels { get; set; }

        public DispatcherTimer MinuteurVague { get; private set; }
        public int Secondes { get; set; }

        private int numeroVague;
        private bool vagueEnCours;

        public GestionVagues(MoteurJeu moteurJeu)
        {
            this.moteurJeu = moteurJeu;
            gestionCarte = moteurJeu.GestionCarte;

            carte = moteurJeu.Carte;
            ile = gestionCarte.Ile;

            EnnemisActuels = new List<Ennemi>();
            Secondes = Constante.TEMPS_ENTRE_VAGUE;

            numeroVague = 0;
            vagueEnCours = false;

            InitMinuteur();
        }

        public void LancerMinuteurVague()
        {
            MinuteurVague.Start();
            moteurJeu.Fenetre.txtStatusVague.Text = "Allez chercher des trésors, vous avez " + Secondes + "s !";
            moteurJeu.Fenetre.txtStatusVague.Visibility = Visibility.Visible;
        }

        private void InitMinuteur()
        {
            MinuteurVague = new DispatcherTimer();
            MinuteurVague.Interval = TimeSpan.FromSeconds(1);
            MinuteurVague.Tick += LancerVague;
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

                MinuteurVague.Stop();
                vagueEnCours = true;

                MinuteurVague.Stop();
                Secondes = Constante.TEMPS_ENTRE_VAGUE;
            }
        }

        private void AjouterVague()
        {
            // Nombre d'ennemis basé sur le numéro de vague (par ex : vague 1 = 3 ennemis, vague 2 = 5, etc.)
            int nombreEnnemis = Math.Min(Constante.VAGUE_MIN_ENNEMI + numeroVague, Constante.VAGUE_MAX_ENNEMI);

            int vieMax = Constante.VIE_BASE_ENNEMI + (numeroVague * Constante.MULTIPLICATEUR_VIE_ENNEMI);
            int degats = Constante.DEGATS_BASE_ENNEMI + numeroVague;
            double tempsRechargement = Math.Max(
                Constante.TEMPS_RECHARGEMENT_MIN_ENNEMI, 
                Constante.TEMPS_RECHARGEMENT_BASE_ENNEMI - (numeroVague * Constante.TEMPS_RECHARGEMENT_MULTIPLICATEUR_ENNEMI));


            AjouterEnnemis(nombreEnnemis, vieMax, degats, Constante.JOUEUR_VITESSE, tempsRechargement);
        }

        public void MettreAJour()
        {
            if (!vagueEnCours) return;
            if (EnnemisActuels.Count == 0)
            {
                vagueEnCours = false;

                MinuteurVague.Start();
            }
        }

        public void AjouterEnnemis(int nombreEnnemis, int vie, int degats, int vitesse, double rechargementCanon)
        {
            for (int i = 0; i < nombreEnnemis; i++)
            {
                Ennemi ennemi = new Ennemi(
                        moteurJeu,
                        vie,
                        degats,
                        vitesse,
                        rechargementCanon
                );

                Point position = gestionCarte.GenererPositionAleatoire(ennemi.CanvaElement.Width, ennemi.CanvaElement.Height, 0, Constante.MARGE_APPARITION_ENNEMI);

                EnnemisActuels.Add(ennemi);
                ennemi.Apparaitre(position);

                Point cible = CalculPointCibleIle(ennemi.Centre);

                ennemi.DefinirCible(cible, ile.Centre);
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
