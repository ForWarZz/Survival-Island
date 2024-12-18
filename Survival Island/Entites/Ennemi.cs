using Survival_Island.Entites.Base;
using Survival_Island.Entites.Navire;
using Survival_Island.Entites.Objets;
using Survival_Island.Outils;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Survival_Island.Entites
{
    public class Ennemi : Bateau
    {
        private Point orientationFinale;
        private Point ciblePrincipale;
        private Point cibleActuelle;

        private bool joueurDansRayon;

        private Joueur joueur;

        public Ennemi(Canvas carte, MoteurJeu moteurJeu, BitmapImage bitmapBateau, int vieMax, int degats, double vitesse, double tempsRechargementCanon) :
            base(carte, moteurJeu, bitmapBateau, Brushes.Red, true, vieMax, degats, vitesse, tempsRechargementCanon)
        {
            joueurDansRayon = false;
            joueur = MoteurJeu.Joueur;
        }

        public void DefinirCible(Point position, Point orientationFinale)
        {
            ciblePrincipale = position;
            cibleActuelle = position;

            this.orientationFinale = orientationFinale;
        }

        public override void Deplacer(double deltaTemps)
        {
            if (cibleActuelle != null)
            {
                if (EstProcheDeCible(cibleActuelle))
                {
                    ChangerOrientation(orientationFinale);
                    CanonActif = true;
                }
                else
                {
                    ChangerOrientation(ciblePrincipale);

                    base.Deplacer(deltaTemps);
                    MettreAJourPositionVie();
                }
            }
        }

        public void VerifierJoueursDansRayon()
        {
            joueurDansRayon = false;

            if (DistanceAvec(joueur.Centre) <= Constante.RAYON_DETECTION_JOUEUR)
            {
                joueurDansRayon = true;
                cibleActuelle = joueur.Centre;
                return;
            }

            if (!joueurDansRayon)
            {
                cibleActuelle = ciblePrincipale;
            }
        }

        private bool EstProcheDeCible(Point position)
        {
            double distance = DistanceAvec(position);
            return distance <= 50;
        }

        private double DistanceAvec(Point position)
        {
            return Math.Sqrt(Math.Pow(PositionX - position.X, 2) + Math.Pow(PositionY - position.Y, 2));
        }
    }
}