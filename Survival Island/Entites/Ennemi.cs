using Survival_Island.Entites.Base;
using Survival_Island.Entites.Navire;
using Survival_Island.Outils;
using Survival_Island.Recherche;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Survival_Island.Entites
{
    public class Ennemi : Bateau
    {
        private EntiteBase cible;

        private RechercheChemin rechercheChemin;

        private Stack<Cellule> chemin;
        private Cellule celluleActuelle;

        private bool cheminEnCoursDeCalcul;

        public Ennemi(Canvas carte, MoteurJeu moteurJeu, BitmapImage bitmapBateau, int vieMax, int degats, double vitesse, double tempsRechargementCanon, EntiteBase cible) : 
            base(carte, moteurJeu, bitmapBateau, true, vieMax, degats, vitesse, tempsRechargementCanon)
        {
            cheminEnCoursDeCalcul = false;
            rechercheChemin = moteurJeu.RechercheChemin;

            this.cible = cible;
        }

        public override async void Apparaitre(double x, double y)
        {
            base.Apparaitre(x, y);

            cheminEnCoursDeCalcul = true;
            double cibleX = cible.PositionX;
            double cibleY = cible.PositionY;

            // Permet de faire le calcul du chemin sans bloquer le jeu
            chemin = await Task.Run(() => 
                        rechercheChemin.TrouverChemin(x, y, cibleX, cibleY));

            cheminEnCoursDeCalcul = false;

            rechercheChemin.AfficherCheminJeu(chemin, Carte);
            celluleActuelle = chemin.Pop();
        }

        public override void Deplacer()
        {
            if (cheminEnCoursDeCalcul || chemin == null || chemin.Count == 0)
            {
                Deplacement = false;
                return;
            }

            // Vérifie si on est proche de la cellule actuelle
            if (EstProcheDeCellule(celluleActuelle))
            {
                celluleActuelle = chemin.Pop();
            }

            // Essayer de faire de l'anticipation pour eviter les arrêts ?
            ChangerOrientation(new Point(celluleActuelle.MondePosX, celluleActuelle.MondePosY));
            if (EstOrienteVers(celluleActuelle))
            {
                base.Deplacer();
                MettreAJourPositionVie();
            }
        }

        private bool EstOrienteVers(Cellule cellule)
        {
            double diffAngle = Math.Abs(AngleCible - AngleActuel);
            return diffAngle <= Constante.TOLERANCE_ANGLE_ROTATION * 2; // Par exemple, 5-10 degrés
        }

        private bool EstProcheDeCellule(Cellule cellule)
        {

            double distance = Math.Sqrt(Math.Pow(PositionX - cellule.MondePosX, 2) + Math.Pow(PositionY - cellule.MondePosY, 2));
            return distance <= 60; // Tolérance
        }
    }
}
