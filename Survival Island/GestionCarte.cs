using Survival_Island.Outils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Controls;
using System.Windows.Media;
using Survival_Island.Entites.Base;
using Survival_Island.Entites.Objets;
using System.Windows.Media.Imaging;
using System.Windows;
using Survival_Island.Entites;

namespace Survival_Island
{
    public class GestionCarte
    {
        private Canvas carte;
        private MoteurJeu moteurJeu;
        
        private BitmapImage bitmapMer;
        private BitmapImage[] bitmapRochers;

        private int tailleCarte;
        private Random random;

        public Obstacle[] Obstacles { get; private set; }
        public Ile Ile { get; private set; }

        public GestionCarte(MoteurJeu moteurJeu)
        {
            carte = moteurJeu.Carte;
            this.moteurJeu = moteurJeu;

            bitmapMer = moteurJeu.GestionImages.Mer;
            bitmapRochers = moteurJeu.GestionImages.Rochers;
            tailleCarte = Constante.NOMBRE_CARREAUX_MER;

            random = new Random();

            InitCarte();
        }


        private void InitCarte()
        {
            carte.Width = tailleCarte * bitmapMer.Width;
            carte.Height = tailleCarte * bitmapMer.Height;

            // Initialisation de la mer en fond
            for (int i = 0; i < tailleCarte; i++)
            {
                for (int j = 0; j < tailleCarte; j++)
                {
                    Image carreauMer = new Image();
                    carreauMer.Source = bitmapMer;
                    carreauMer.Width = bitmapMer.Width;
                    carreauMer.Height = bitmapMer.Height;

                    // Retire l'effet quadrilage des bords
                    RenderOptions.SetEdgeMode(carreauMer, EdgeMode.Aliased);

                    Canvas.SetLeft(carreauMer, j * bitmapMer.Width);
                    Canvas.SetTop(carreauMer, i * bitmapMer.Height);

                    carte.Children.Add(carreauMer);
                }
            }
        }

        public void InitRochers()
        {
            Obstacles = new Obstacle[Constante.NOMBRE_ROCHERS_CARTE];

            for (int i = 0; i < Obstacles.Length; i++)
            {
                Image rocher = new Image();
                BitmapImage randomRocher = bitmapRochers[random.Next(0, bitmapRochers.Length)];

                double angleRotation = random.Next(0, 361);
                double multiplicateurTaille = Constante.MULTIPLICATEUR_TAILLE_ROCHER + random.NextDouble();

                rocher.Source = randomRocher;
                rocher.Width = randomRocher.Width * multiplicateurTaille;
                rocher.Height = randomRocher.Height * multiplicateurTaille;
                rocher.RenderTransform = new RotateTransform(angleRotation, rocher.Width / 2, rocher.Height / 2);

                Point position = GenererPositionAleatoire(rocher.Width, rocher.Height, angleRotation);

                Obstacle obstacle = new Obstacle(carte, rocher);
                obstacle.Apparaitre(position);

                Obstacles[i] = obstacle;
            }
        }

        public void InitIle()
        {
            Ile = new Ile(moteurJeu);
            Ile.Apparaitre();
        }

        public Point GenererPositionAleatoire(double largeur, double hauteur, double angleRotation = 0, int marge = 0)
        {
            double posX, posY;
            bool positionValide;

            int carteLargeur = (int)carte.Width;
            int carteHauteur = (int)carte.Height;

            do
            {
                if (marge > 0)
                {
                    // Générer la position sur un bord en tenant compte de la marge
                    int bord = random.Next(0, 4); // 0 = gauche, 1 = droite, 2 = haut, 3 = bas

                    switch (bord)
                    {
                        case 0: // Bord gauche
                            posX = random.Next(0, marge);
                            posY = random.Next(0, carteHauteur - (int)hauteur);
                            break;
                        case 1: // Bord droit
                            posX = random.Next(carteLargeur - marge, carteLargeur - (int)largeur);
                            posY = random.Next(0, carteHauteur - (int)hauteur);
                            break;
                        case 2: // Bord haut
                            posX = random.Next(0, carteLargeur - (int)largeur);
                            posY = random.Next(0, marge);
                            break;
                        default: // Bord bas
                            posX = random.Next(0, carteLargeur - (int)largeur);
                            posY = random.Next(carteHauteur - marge, carteHauteur - (int)hauteur);
                            break;
                    }
                }
                else
                {
                    posX = random.Next(0, carteLargeur - (int)largeur);
                    posY = random.Next(0, carteHauteur - (int)hauteur);
                }

                // Vérifier la validité de la position
                Collision collision = new Collision(posX, posY, largeur, hauteur, angleRotation);
                positionValide = CheckPositionValide(collision);
            } while (!positionValide);

            return new Point(posX, posY);
        }

        private bool CheckPositionValide(Collision collision)
        {
            if (Calcul.DistanceAvec(collision.Centre, Ile.Centre) < Constante.MARGE_ILE)
            {
                Console.WriteLine("DEBUG: Trop proche de l'île");
                return false;
            }

            foreach (Obstacle obstacleDejaPresent in Obstacles)
            {
                if (obstacleDejaPresent != null && obstacleDejaPresent.EnCollisionAvec(collision))
                {
                    Console.WriteLine("DEBUG: Collision avec obstacle");
                    return false;
                }
            }

            foreach (ObjetRecompense objetsBonusDejaPresent in moteurJeu.ObjetsBonus)
            {
                if (objetsBonusDejaPresent != null && objetsBonusDejaPresent.EnCollisionAvec(collision))
                {
                    Console.WriteLine("DEBUG: Collision avec objet bonus");
                    return false;
                }
            }

            foreach (Ennemi ennemi in moteurJeu.GestionVagues.EnnemisActuels)
            {
                if (ennemi.EnCollisionAvec(collision))
                {
                    //Console.WriteLine("DEBUG: Collision avec ennemi");
                    return false;
                }
            }

            return true;
        }
    }
}
