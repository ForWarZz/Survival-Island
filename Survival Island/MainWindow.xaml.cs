using Survival_Island.carte;
using Survival_Island.joueur;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Survival_Island
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private int NOMBRE_IMAGE_MER, IM_MER_LARG, IM_MER_HAUT;

        private Image[] mer;

        private BitmapImage bitmapMer, bitmapTresor;
        private BitmapImage[] bitmapRochers;

        private DispatcherTimer objetBonusMinuteur;

        private Joueur joueur;
        private Ile ile;

        private bool jouer = false;

        private Random random = new Random();

/*        private List<Item> listeItem = new List<Item>();
        private List<Ennemi> listeEnnemis;*/

        private List<Boulet> boulets;
        private List<Objet> objetsBonus;
        private Obstacle[] obstacles;

        private double cameraX; // Position caméra (X)
        private double cameraY; // Position caméra (Y)

        public MainWindow()
        {
            InitializeComponent();

            InitBitmaps();
            InitCarteSize();
            InitCarte();
        }

        private void InitBonusMinuteur()
        { 
            objetBonusMinuteur = new DispatcherTimer();
            objetBonusMinuteur.Interval = TimeSpan.FromSeconds(10);
            objetBonusMinuteur.Tick += GenererBonus;
           /* objetBonusMinuteur.Start();*/
        }

        private void GenererBonus(object? sender, EventArgs e)
        {
            int nbBonus = random.Next(1, 4);

            for (int i = 0; i < nbBonus; i++)
            {
                bool positionValide = true;
                double posX, posY;

                // On check la position par rapport aux autres elements
                do
                {
                    posX = random.Next(0, (int)(carte.Width - bitmapTresor.Width));
                    posY = random.Next(0, (int)(carte.Height - bitmapTresor.Height));

                    Rect rect = new Rect(posX, posY, bitmapTresor.Width, bitmapTresor.Height);

                    // Vérification des collisions avec l'île et les autres obstacles/bonus
                    positionValide = CheckPosition(rect);
                } while (!positionValide);

                double multiplicateur = 0.5 + random.NextDouble();
                int objetLargeur = (int)(bitmapTresor.Width * multiplicateur);
                int objetHauteur = (int)(bitmapTresor.Height * multiplicateur);

                Objet objet = new Objet(carte, bitmapTresor, objetLargeur, objetHauteur, Constante.BASE_COFFRE_VIE, Constante.BASE_COFFRE_EXPERIENCE);
                objet.Apparaitre(posX, posY);

                objetsBonus.Add(objet);

                Console.WriteLine("Pos X: " + posX + " pos y: " + posY);
            }
        }

        private void InitRochers()
        {
            obstacles = new Obstacle[Constante.NOMBRE_CAILLOUX_CARTE];

            for (int i = 0; i < obstacles.Length; i++)
            {
                Console.WriteLine("Creating rocher " + i);
                Image rocher = new Image();
                BitmapImage randomRocher = bitmapRochers[random.Next(0, bitmapRochers.Length)];

                rocher.Source = randomRocher;
                rocher.Width = randomRocher.Width * (0.5 + random.NextDouble());
                rocher.Height = randomRocher.Height * (0.5 + random.NextDouble());
                rocher.RenderTransform = new RotateTransform(random.Next(0, 361), rocher.Width / 2, rocher.Height / 2);

                Console.WriteLine("Width : " + rocher.Width + ", Height: " + rocher.Height);

                double posX, posY;
                bool positionValide;

                // Vérification de la position
                do
                { 
                    posX = random.Next(0, (int)(carte.Width - rocher.Width));
                    posY = random.Next(0, (int)(carte.Height - rocher.Height));

                    Rect rect = new Rect(posX, posY, rocher.Width, rocher.Height);

                    // Vérification des collisions avec l'île et les autres obstacles
                    positionValide = CheckPosition(rect);
                } while (!positionValide);

                // Création et ajout de l'obstacle à la carte
                Obstacle obstacle = new Obstacle(carte, rocher);
                obstacle.Apparaitre(posX, posY);

                Console.WriteLine("Rocher " + i + " created at " + posX + ", " + posY);

                obstacles[i] = obstacle;
            }
        }

        private bool CheckPosition(Rect rect)
        {
            // Vérification si le rectangle entre en collision avec l'île
            if (ile.EnCollisionAvec(rect))
            {
                return false;
            }

            // Vérification si le rectangle entre en collision avec d'autres obstacles
            foreach (Obstacle obstacleDejaPresent in obstacles)
            {
                if (obstacleDejaPresent != null && obstacleDejaPresent.EnCollisionAvec(rect))
                {
                    return false;
                }
            }

            // Vérification si le rectangle entre en collision avec des objet bonus
            foreach (Objet objetsBonusDejaPresent in objetsBonus)
            {
                if (objetsBonusDejaPresent != null && objetsBonusDejaPresent.EnCollisionAvec(rect))
                {
                    return false;
                }
            }

            return true;
        }




        /*        private void InitEnemies()
                {
                    listeEnnemis = new List<Ennemi>();

                    for (int i = 0; i < 5; i++) // Ajouter 5 ennemis
                    {
                        double posX = rnd.Next(0, (int)carte.Width);
                        double posY = rnd.Next(0, (int)carte.Height);
                        Vector position = new Vector(posX, posY);

                        Ennemi ennemi = new Ennemi(carte, joueur, position);
                        listeEnnemis.Add(ennemi);
                    }

                }*/
        /*        private void AjoutItems(List<Item> listeItem, int nbItemAjout, string[] listeCheminAjout ,int posXmin, int posXmax, int posYmin, int posYmax, int longCoteImageMin, int longCoteImageMax, bool rotation)
                {
                    int posX,posY,longCoteImage, longListeChemin = listeCheminAjout.Length;
                    int indChemin;
                    int nbitem = listeItem.Count;

                    for (int i =nbitem ; i < nbitem+nbItemAjout; i++)
                    {
                        longCoteImage=random.Next(longCoteImageMin, longCoteImageMax);
                        posX = random.Next(posXmin,posXmax);
                        posY = random.Next(posYmin,posYmax);
                        indChemin = random.Next(0,longListeChemin);

                        listeItem.Add( new Item(posX, posY, 90,30, listeCheminAjout[indChemin],longCoteImage,longCoteImage,carte));

                        if (rotation)
                        {
                            listeItem[i].image.RenderTransform = new RotateTransform(random.Next(0, 361), longCoteImage, longCoteImage);
                        }
                    }
                }*/


        private void InitBoucleJeu()
        {
            CompositionTarget.Rendering += Jeu;
        }

        private void Jeu(object? sender, EventArgs e)
        {
            CheckDeplacement();

            if (joueur.canonActif)
                joueur.TirerBoulet();

            DeplacerBoulets();

/*            foreach (var ennemi in listeEnnemis)
            {
                ennemi.MettreAJour();
            }

            joueur.CheckCollisions(listeEnnemis);*/
        }

        private void CheckDeplacement()
        {
            double vitesse = joueur.caracteristique.vitesse;

            if (joueur.deplacement)
            {
                Vector orientation = joueur.orientation;
                DeplaceCamera(orientation.X * vitesse, orientation.Y * vitesse);   // On inverse le vecteur pour déplacer le monde dans le sens opposé
            }
        }

        private void InitBitmaps()
        {
            bitmapMer = new BitmapImage(new Uri(Chemin.IMAGE_MER));
            bitmapRochers = [
                    new BitmapImage(new Uri(Chemin.IMAGE_ROCHER1)),
                    new BitmapImage(new Uri(Chemin.IMAGE_ROCHER2)),
                    new BitmapImage(new Uri(Chemin.IMAGE_ROCHER3)),
                ];

            bitmapTresor = new BitmapImage(new Uri(Chemin.IMAGE_TRESOR));
        }

        private void InitCarteSize()
        {
            IM_MER_LARG = (int)carte.Width / (int)bitmapMer.Width;
            IM_MER_HAUT = (int)carte.Height / (int)bitmapMer.Height;
            NOMBRE_IMAGE_MER = IM_MER_HAUT * IM_MER_LARG;

            mer = new Image[NOMBRE_IMAGE_MER];
        }

        private void InitCarte()
        {
            /// Initialisation de la mer en fond
            for (int i = 0; i < IM_MER_HAUT; i++)
            {
                for (int j = 0; j < IM_MER_LARG; j++)
                {
                    mer[i] = new Image();
                    mer[i].Source = bitmapMer;
                    mer[i].Width = bitmapMer.Width;
                    mer[i].Height = bitmapMer.Height;

                    Canvas.SetLeft(mer[i], j * bitmapMer.Width);
                    Canvas.SetTop(mer[i], i * bitmapMer.Height);

                    carte.Children.Add(mer[i]);
                }
            }
        }

        private void LancerJeu()
        {
            hudJoueur.Visibility = Visibility.Visible;

            boulets = new List<Boulet>();
            objetsBonus = new List<Objet>();

            InitCamera();

            ile = new Ile(carte, progressVieIle, txtVieIle);
            ile.Apparaitre();

            joueur = new Joueur(carte, progressVieNavire, txtVieNavire, boulets);
            joueur.Apparaitre();

            InitRochers();
           /* InitBonusMinuteur();*/

            InitBoucleJeu();


            //Fonction de test des items.

            //AjoutItems(listeItem, 70, [Chemin.IMAGE_TRESOR], 0, 4000, 0, 4000, 20, 70, false);
            //AjoutItems(listeItem, 70, [Chemin.IMAGE_ROCHER1, Chemin.IMAGE_ROCHER2, Chemin.IMAGE_ROCHER3], 0, 4000, 0, 4000, 50, 200, true);
            /*InitEnemies();*/
        }

        private void DeplaceCamera(double deltaX, double deltaY)
        {
            cameraX += deltaX;
            cameraY += deltaY;

            // Restreindre les limites de la caméra
            cameraX = Math.Max(0, Math.Min(cameraX, carte.Width - Fenetre.ActualWidth));
            cameraY = Math.Max(0, Math.Min(cameraY, carte.Height - Fenetre.ActualHeight));

            // Appliquer le décalage au Canvas (monde)
            Canvas.SetLeft(carte, -cameraX);
            Canvas.SetTop(carte, -cameraY);

            double bateauX = Canvas.GetLeft(joueur.element);
            double bateauY = Canvas.GetTop(joueur.element);

            // Appliquer le décalage au joueur
            Canvas.SetLeft(joueur.element, bateauX + deltaX);
            Canvas.SetTop(joueur.element, bateauY + deltaY);
        }

        private void InitCamera()
        {
            cameraX = carte.Width / 2 - Fenetre.ActualWidth / 2;
            cameraY = carte.Height / 2 - Fenetre.ActualHeight / 2;

            Canvas.SetLeft(carte, -cameraX);
            Canvas.SetTop(carte, -cameraY);

            Console.WriteLine("Camera X: " + cameraX + ", Camera Y: " + cameraY);
        }

        /*
                private void DeplaceMonde(double x, double y)
                {
                    foreach (UIElement element in carte.Children)
                    {
                        // Ignorer le joueur et autres éléments ne devant pas bouger
                        if (element == joueur.element)
                            continue;

                        double positionX = Canvas.GetLeft(element);
                        Canvas.SetLeft(element, positionX + x);

                        double positionY = Canvas.GetTop(element);
                        Canvas.SetTop(element, positionY + y);
                    }
                }*/

        private void DeplacerBoulets()
        {
            // On parcourt la liste des boulets à l'envers pour pouvoir supprimer des éléments
            for (int i = 0; i < boulets.Count; i++)
            {
                Boulet boulet = boulets[i];
                double bouletX = Canvas.GetLeft(boulet.element);
                double bouletY = Canvas.GetTop(boulet.element);

                bouletX += boulet.direction.X * Constante.VITESSE_BOULET; // Vitesse du boulet
                bouletY += boulet.direction.Y * Constante.VITESSE_BOULET;

                Canvas.SetLeft(boulet.element, bouletX);
                Canvas.SetTop(boulet.element, bouletY);

                // Supprimer le boulet si hors écran
                if (bouletX < 0 || bouletY < 0 ||
                    bouletX > carte.ActualWidth + boulet.element.Width || bouletY > carte.ActualHeight + boulet.element.Width)
                {
                    carte.Children.Remove(boulet.element);
                    boulets.RemoveAt(i);
                }
            }

            if (joueur.tempsDernierBoulet > 0)
            {
                joueur.tempsDernierBoulet -= 1.0 / 60.0;   // On suppose 60 FPS, pour convertir
            }

        }

        private void Fenetre_KeyUp(object sender, KeyEventArgs e)
        {
            if (jouer)
            {
                if (e.Key == Key.Z)
                {
                    joueur.deplacement = false;
                }
            }
        }

        private void Fenetre_KeyDown(object sender, KeyEventArgs e)
        {
            if (jouer)
            {
                if (e.Key == Key.Z)
                {
                    joueur.deplacement = true;
                }
            }
        }


        private void Fenetre_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
                joueur.canonActif = true;
        }

        private void Fenetre_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
                joueur.canonActif = false;
        }

        private void Fenetre_MouseMove(object sender, MouseEventArgs e)
        {

            if (jouer)
            { 
                Point positionSouris = e.GetPosition(carte);
                joueur.UpdateOrientation(positionSouris);
            }

        }

        private void btnFermerJeu_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Êtes vous sûr de vouloir quitter le jeu ?", "Fermer le jeu", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
                Application.Current.Shutdown();
        }

        private void btnJouer_Click(object sender, RoutedEventArgs e)
        {
            menuAccueil.Visibility = Visibility.Hidden;
            jouer = true;
            LancerJeu();
        }
    }
}