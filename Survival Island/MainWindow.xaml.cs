using Survival_Island.carte;
using Survival_Island.carte.objets;
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

        public MainWindow()
        {
            InitializeComponent();

            InitBitmaps();
            InitCarte();
        }

        private void InitBonusMinuteur()
        { 
            objetBonusMinuteur = new DispatcherTimer();
            objetBonusMinuteur.Interval = TimeSpan.FromMinutes(1);
            objetBonusMinuteur.Tick += GenererBonus;
            objetBonusMinuteur.Start();
        }

        private void GenererBonus(object? sender, EventArgs e)
        {
            int nbBonus = random.Next(0, 4);

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

                Objet objet = new Objet(carte, bitmapTresor, objetLargeur, objetHauteur, Constante.BASE_COFFRE_VIE, Constante.BASE_COFFRE_EXPERIENCE, true);
                objet.Apparaitre(posX, posY);

                objetsBonus.Add(objet);
            }
        }

        private void InitRochers()
        {
            obstacles = new Obstacle[Constante.NOMBRE_CAILLOUX_CARTE];

            for (int i = 0; i < obstacles.Length; i++)
            {
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
                    posY = random.Next(0, (int)(carte.Width - rocher.Height));

                    Rect rect = new Rect(posX, posY, rocher.Width, rocher.Height);

                    // Vérification des collisions avec l'île et les autres obstacles
                    positionValide = CheckPosition(rect);
                } while (!positionValide);

                // Création et ajout de l'obstacle à la carte
                Obstacle obstacle = new Obstacle(carte, rocher);
                obstacle.Apparaitre(posX, posY);

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

        private void InitBoucleJeu()
        {
            CompositionTarget.Rendering += Jeu;
        }

        private void Jeu(object? sender, EventArgs e)
        {
            CheckDeplacement();
            DeplacerBoulets();

            if (joueur.canonActif)
                joueur.TirerBoulet();
        }

        private void CheckDeplacement()
        {
            double vitesse = joueur.caracteristique.vitesse;

            if (joueur.deplacement)
            {
                Vector orientation = joueur.orientation;
                joueur.DeplaceBateau(orientation.X * vitesse, orientation.Y * vitesse);
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

        private void InitCarte()
        {
            mer = new Image[Constante.NOMBRE_CARREAUX_MER];
            carte.Width = mer.Length * bitmapMer.Width;
            carte.Height = mer.Length * bitmapMer.Height;

            /// Initialisation de la mer en fond
            for (int i = 0; i < mer.Length; i++)
            {
                for (int j = 0; j < mer.Length; j++)
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

            ile = new Ile(carte, progressVieIle, txtVieIle);
            ile.Apparaitre();

            joueur = new Joueur(carte, this, boulets);
            joueur.Apparaitre();

            InitRochers();
            InitBonusMinuteur();

            InitBoucleJeu();


            //Fonction de test des items.

            //AjoutItems(listeItem, 70, [Chemin.IMAGE_TRESOR], 0, 4000, 0, 4000, 20, 70, false);
            //AjoutItems(listeItem, 70, [Chemin.IMAGE_ROCHER1, Chemin.IMAGE_ROCHER2, Chemin.IMAGE_ROCHER3], 0, 4000, 0, 4000, 50, 200, true);
            /*InitEnemies();*/
        }

/*        private void DeplaceCamera(double deltaX, double deltaY)
        {
            cameraX += deltaX;
            cameraY += deltaY;

            // Restreindre les limites de la caméra
            cameraX = Math.Max(0, Math.Min(cameraX, carte.Width - Fenetre.ActualWidth));
            cameraY = Math.Max(0, Math.Min(cameraY, carte.Height - Fenetre.ActualHeight));

            double bateauX = Canvas.GetLeft(joueur.canvaElement);
            double bateauY = Canvas.GetTop(joueur.canvaElement);

            // Appliquer le décalage au Canvas (monde)
            Canvas.SetLeft(carte, -cameraX);
            Canvas.SetTop(carte, -cameraY);

            joueur.DeplaceBateau(deltaX, deltaY);
        }*/

/*        private void InitCamera()
        {
            cameraX = carte.Width / 2 - Fenetre.ActualWidth / 2;
            cameraY = carte.Height / 2 - Fenetre.ActualHeight / 2;

            Canvas.SetLeft(carte, -cameraX);
            Canvas.SetTop(carte, -cameraY);

            Console.WriteLine("Camera X: " + cameraX + ", Camera Y: " + cameraY);
        }*/

        /*
                private void DeplaceMonde(double x, double y)
                {
                    foreach (UIElement canvaElement in carte.Children)
                    {
                        // Ignorer le joueur et autres éléments ne devant pas bouger
                        if (canvaElement == joueur.canvaElement)
                            continue;

                        double positionX = Canvas.GetLeft(canvaElement);
                        Canvas.SetLeft(canvaElement, positionX + x);

                        double positionY = Canvas.GetTop(canvaElement);
                        Canvas.SetTop(canvaElement, positionY + y);
                    }
                }*/

        private void DeplacerBoulets()
        {
            // On parcourt la liste des boulets à l'envers pour pouvoir supprimer des éléments
            for (int i = 0; i < boulets.Count; i++)
            {
                Boulet boulet = boulets[i];
                double bouletX = Canvas.GetLeft(boulet.canvaElement);
                double bouletY = Canvas.GetTop(boulet.canvaElement);

                bouletX += boulet.direction.X * Constante.VITESSE_BOULET; // Vitesse du boulet
                bouletY += boulet.direction.Y * Constante.VITESSE_BOULET;

                Canvas.SetLeft(boulet.canvaElement, bouletX);
                Canvas.SetTop(boulet.canvaElement, bouletY);

                // Supprimer le boulet si hors écran
                if (bouletX < 0 || bouletY < 0 ||
                    bouletX > carte.Width + boulet.canvaElement.Width || bouletY > carte.Height + boulet.canvaElement.Width)
                {
                    carte.Children.Remove(boulet.canvaElement);
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