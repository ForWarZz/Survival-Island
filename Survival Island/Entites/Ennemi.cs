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

        private List<EntiteBase> obstaclesCaches;

        public Ennemi(MoteurJeu moteurJeu, int vieMax, int degats, double vitesse, double tempsRechargementCanon) :
            base(moteurJeu, moteurJeu.GestionImages.BateauEnnemi, Brushes.Red, true, vieMax, degats, vitesse, tempsRechargementCanon)
        {
            joueurDansRayon = false;
            joueur = MoteurJeu.Joueur;
            obstaclesCaches = new List<EntiteBase>();
        }

        public void DefinirCible(Point position, Point orientationFinale)
        {
            ciblePrincipale = position;
            cibleActuelle = position;
            this.orientationFinale = orientationFinale;
        }

        public override void Deplacer(double deltaTemps)
        {
            VerifierJoueursDansRayon();

            if (joueurDansRayon)
            {
                cibleActuelle = joueur.Centre;
                ChangerOrientation(joueur.Centre);
                CanonActif = true;
            }
            else
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

            // Réapparaître les obstacles cachés si l'ennemi est assez loin
            ReapparaitreObstaclesCaches();
        }

        protected override bool PeutAllerVers(double nouvellePosX, double nouvellePosY)
        {
            Collision nouvelleCollision = new Collision(nouvellePosX, nouvellePosY, CanvaElement.Width, CanvaElement.Height, AngleRotation());

            if (nouvelleCollision.CollisionDevantAvec(MoteurJeu.GestionCarte.Ile.CollisionRectangle))
                return false;

            if (nouvelleCollision.EnCollisionAvec(MoteurJeu.Joueur.CollisionRectangle))
                return false;

            foreach (Obstacle obstacle in MoteurJeu.GestionCarte.Obstacles)
            {
                if (nouvelleCollision.CollisionDevantAvec(obstacle.CollisionRectangle))
                {
                    CacherEntite(obstacle);
                    return true; // L'ennemi peut continuer, car l'obstacle est "caché"
                }
            }

            foreach (ObjetRecompense objetBonus in MoteurJeu.ObjetsBonus)
            {
                if (nouvelleCollision.CollisionDevantAvec(objetBonus.CollisionRectangle))
                {
                    CacherEntite(objetBonus);
                    return true; // L'ennemi peut continuer, car l'obstacle est "caché"
                }
            }

            return true;
        }

        private void CacherEntite(EntiteBase entite)
        {
            if (!obstaclesCaches.Contains(entite))
            {
                entite.CanvaElement.Visibility = Visibility.Hidden;
                obstaclesCaches.Add(entite);
            }
        }

        private void ReapparaitreObstaclesCaches()
        {
            for (int i = obstaclesCaches.Count - 1; i >= 0; i--)
            {
                EntiteBase entite = obstaclesCaches[i];
                if (DistanceAvec(entite.Position) > Constante.DISTANCE_EVASION)
                {
                    entite.CanvaElement.Visibility = Visibility.Visible;
                    obstaclesCaches.RemoveAt(i);
                }
            }
        }

        public void VerifierJoueursDansRayon()
        {
            joueurDansRayon = false;

            if (DistanceAvec(joueur.Centre) <= Constante.RAYON_DETECTION_JOUEUR)
            {
                joueurDansRayon = true;
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
            return distance <= Constante.TOLERANCE_CIBLE_ENNEMI;
        }

        private double DistanceAvec(Point position)
        {
            return Math.Sqrt(Math.Pow(PositionX - position.X, 2) + Math.Pow(PositionY - position.Y, 2));
        }

        protected override void BouletTire() 
        { }
    }
}