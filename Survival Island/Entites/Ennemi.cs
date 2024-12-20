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

        private bool orientationEnCours;
        private bool orientationFinaleEnCours;

        private bool joueurDansRayon;
        private List<EntiteBase> obstaclesCaches;
        private Joueur joueur;

        public Ennemi(MoteurJeu moteurJeu, int vieMax, int degats, double vitesse, double tempsRechargementCanon) :
            base(moteurJeu, moteurJeu.GestionImages.BateauEnnemi, Brushes.Red, true, vieMax, degats, vitesse, tempsRechargementCanon)
        {
            joueurDansRayon = false;
            joueur = MoteurJeu.Joueur;
            obstaclesCaches = new List<EntiteBase>();

            orientationEnCours = false;
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

                ChangerOrientation(cibleActuelle);
                CanonActif = true;

                orientationFinaleEnCours = false;
            }
            else
            {
                //Console.WriteLine("DEBUG: Joueur pas dans rayon, depalcement vers cible");

                if (EstProcheDeCible(ciblePrincipale))
                {
                    //Console.WriteLine("DEBUG: Ennemi arrivé à sa cible principale.");
                    CanonActif = true;
                }
                else
                {
                    if (AngleCible == AngleActuel && !orientationFinaleEnCours)
                    {
                        //Console.WriteLine("DEBUG: Orientation finale en cours");

                        ChangerOrientation(orientationFinale);
                        orientationFinaleEnCours = true;
                    } else if (!orientationEnCours)
                    {
                        //Console.WriteLine("DEBUG: Orientation cible en cours");

                        ChangerOrientation(cibleActuelle);
                        orientationEnCours = true;
                    } else
                    {
                        //Console.WriteLine("DEBUG: Orientation bonne, deplacement");

                        base.Deplacer(deltaTemps);
                        MettreAJourPositionVie();
                    }
                }
            }

            ReapparaitreObstaclesCaches();
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
                if (Calcul.DistanceAvec(Position, entite.Position) > Constante.DISTANCE_EVASION)
                {
                    entite.CanvaElement.Visibility = Visibility.Visible;
                    obstaclesCaches.RemoveAt(i);
                }
            }
        }


        public void VerifierJoueursDansRayon()
        {
            if (joueur.EstMort)
            {
                joueurDansRayon = false;
                cibleActuelle = ciblePrincipale;

                ChangerOrientation(orientationFinale);
                return;
            }

            joueurDansRayon = false;
            if (Calcul.DistanceAvec(Centre, joueur.Centre) <= Constante.RAYON_DETECTION_JOUEUR)
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
            double distance = Calcul.DistanceAvec(Centre, position);
            return distance <= Constante.TOLERANCE_CIBLE_ENNEMI;
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
    }
}