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
        private BitmapImage bitmapMer, bitmapIle;

        public MainWindow()
        {
            InitializeComponent();

            InitBitmaps();
            InitCarteSize();
            InitCarte();
        }

        private void InitBitmaps()
        {
            bitmapMer = new BitmapImage(new Uri(Chemin.IMAGE_MER));
            bitmapIle = new BitmapImage(new Uri(Chemin.IMAGE_ILE));
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
            Image ile = new Image();

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
            // Définir ses stats
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
            LancerJeu();
        }
    }
}