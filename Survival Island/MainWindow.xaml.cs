using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Survival_Island
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private int NOMBRE_IMAGE_MER, IM_MER_LARG, IM_MER_HAUT;

        private Image[] laMer;
        private BitmapImage bitmapMer, bitmapIle, bitmapBateauRouge, bitmapBateauVert, bitmapIleC;
        private Image ile;

        public Image BateauRouge = new Image();

        public Image BateauVert = new Image();

        private DispatcherTimer MinuterieGlobal;

        Joueur joueur;

        private bool boutonJouerClique = false;
        public MainWindow()
        {
            InitializeComponent();

            InitBitmaps();
            InitCarteSize();
            InitCarte();
        }

        private void InitMinuterie()
        {

            MinuterieGlobal = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(6) // Environ 60 FPS
            };
            MinuterieGlobal.Tick += Jeu;
            MinuterieGlobal.Start();
        }

        private void Jeu(object? sender, EventArgs e)
        {


            if (joueur.CannonActif && joueur.TempsDernierShoot <= 0)
            {
                joueur.Shoot();
                joueur.TempsDernierShoot = joueur.VitesseRechargement;
            }
            joueur.TempsDernierShoot -= 1;
            // Calculer l'angle cible entre le centre de l'image et la souris
            double targetAngle = joueur.CalculateAngle(joueur.PositionSouris);

            // Interpoler vers l'angle cible
            joueur.AngleActuelDuBateau = joueur.InterpolateAngle(joueur.AngleActuelDuBateau, targetAngle, joueur.RotationSpeed);

            // Appliquer la rotation à l'image
            joueur.RotateImage(joueur.AngleActuelDuBateau);

            this.Focus();
        }

        private void InitBitmaps()
        {
            bitmapMer = new BitmapImage(new Uri(Chemin.IMAGE_MER));
            bitmapIle = new BitmapImage(new Uri(Chemin.IMAGE_ILE));
            bitmapBateauRouge = new BitmapImage(new Uri(Chemin.IMAGE_BATEAU_ROUGE));
            bitmapBateauVert = new BitmapImage(new Uri(Chemin.IMAGE_BATEAU_VERT));
        }
        private void InitBateaux()
        {
            BateauRouge.Source = bitmapBateauRouge;
            BateauRouge.Width = 60;
            BateauRouge.Height = 100;
            Canvas.SetLeft(BateauRouge, 1650);
            Canvas.SetTop(BateauRouge, 1600);

            BateauVert.Source = bitmapBateauVert;
            BateauVert.Width = 60;
            BateauVert.Height = 100;
            Canvas.SetLeft(BateauVert, 1650);
            Canvas.SetTop(BateauVert, 1700);
        }
        private void InitCarteSize()
        {
            IM_MER_LARG = (int)carteBackground.Width / (int)bitmapMer.Width;
            IM_MER_HAUT = (int)carteBackground.Height / (int)bitmapMer.Height;
            NOMBRE_IMAGE_MER = IM_MER_HAUT * IM_MER_LARG;

            laMer = new Image[NOMBRE_IMAGE_MER];
        }

        private void InitCarte()
        {
            /// Initialisation de la mer en fond
            for (int i = 0; i < IM_MER_HAUT; i++)
            {
                for (int j = 0; j < IM_MER_LARG; j++)
                {
                    laMer[i] = new Image();
                    laMer[i].Source = bitmapMer;
                    laMer[i].Width = bitmapMer.Width;
                    laMer[i].Height = bitmapMer.Height;

                    Canvas.SetLeft(laMer[i], j * bitmapMer.Width);
                    Canvas.SetTop(laMer[i], i * bitmapMer.Height);

                    carteBackground.Children.Add(laMer[i]);
                }
            }
        }
        private void InitIle()
        {
            /// REMPLACER CE CODE PAR LA CLASSE DE L'ILE
            ile = new Image();

            ile.Source = bitmapIle;
            ile.Width = bitmapIle.PixelWidth;
            ile.Height = bitmapIle.PixelHeight;

            Canvas.SetLeft(ile, 1750);
            Canvas.SetTop(ile, 1700);

            carteBackground.Children.Add(ile);
        }

        private void LancerJeu()
        {
            hudJoueur.Visibility = Visibility.Visible;
            InitIle();
            // Faire spawn le navire du joueur
            InitBateaux();

            joueur = new Joueur(BateauRouge, carteBackground);
            ChoisirCouleurBateau();
            InitMinuterie();

        }
        private void ChoisirCouleurBateau()
        {
            // Afficher une boîte de dialogue pour le choix de la couleur
            MessageBoxResult result = MessageBox.Show(
                "Choisissez la couleur de votre bateau :\n\n" +
                "Cliquez sur Oui pour Rouge ou sur Non pour Vert.",
                "Choix de la couleur du bateau",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            // Appliquer la couleur en fonction du choix
            if (result == MessageBoxResult.Yes)
            {
                joueur.RedShip = BateauRouge; // Couleur rouge

                carteBackground.Children.Add(BateauRouge);
            }
            else if (result == MessageBoxResult.No)
            {
                joueur.RedShip = BateauVert; // Couleur verte

                carteBackground.Children.Add(BateauVert);
            }
        }

        private void DeplaceMonde(double x, double y)
        {
            foreach (UIElement element in carteBackground.Children)
            {
                // Ignorer le joueur et autres éléments ne devant pas bouger
                double positionX = Canvas.GetLeft(element);
                Canvas.SetLeft(element, positionX + x);

                double positionY = Canvas.GetTop(element);
                Canvas.SetTop(element, positionY + y);
            }
        }

        private void Fenetre_KeyUp(object sender, KeyEventArgs e)
        {
            if (boutonJouerClique)
            {
                if (e.Key == Key.D)
                {
                    joueur.MouvementDroitBolleen = false; // Stop moving right
                }
                else if (e.Key == Key.Q)
                {
                    joueur.MouvementGaucheBolleen = false; // Stop moving left
                }
                else if (e.Key == Key.Z)
                {
                    joueur.MouvementHautBolleen = false; // Stop moving up
                }
                else if (e.Key == Key.S)
                {
                    joueur.MouvementBasBolleen = false; // Stop moving down
                }
            }
        }


        private void Fenetre_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (boutonJouerClique)
                joueur.CannonActif = true; // Set cannon active when mouse button is pressed

        }

        private void Fenetre_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (boutonJouerClique)
                joueur.CannonActif = false; // Set cannon inactive when mouse button is released
        }

        private void Fenetre_MouseMove(object sender, MouseEventArgs e)
        {
            if (boutonJouerClique)
            {
                // Récupérer la position de la souris
                joueur.PositionSouris = e.GetPosition(this);

            }

        }

        private void Fenetre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                DeplaceMonde(5, 0);
            }
            else if (e.Key == Key.Right)
            {
                DeplaceMonde(-5, 0);
            }
            else if (e.Key == Key.Up)
            {
                DeplaceMonde(0, 5);
            }
            else if (e.Key == Key.Down)
            {
                DeplaceMonde(0, -5);
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
            boutonJouerClique = true;
            LancerJeu();
        }
    }
}