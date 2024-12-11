using Survival_Island.ile;
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
        private int NOMBRE_IMAGE_MER, IM_MER_LARG, IM_MER_HAUT, NOMBRE_CAILLOUX = 20;

        private Image[] laMer, lesCailloux;

        private BitmapImage bitmapMer, bitmapRocher1, bitmapRocher2, bitmapRocher3;
        private BitmapImage[] imgCailloux;

        private DispatcherTimer minuterieJeu;
        private bool deplacementHaut, deplacementBas, deplacementDroite, deplacementGauche;

        private Joueur joueur;
        private bool canonActif = false;

        private Ile ile;

        private bool jouer = false;

        private Random rnd = new Random();
        private Random random = new Random();

        private List<Item> listeItem = new List<Item>();

        public MainWindow()
        {
            InitializeComponent();

            InitBitmaps();
            InitCarteSize();
            InitCarte();


        }

        private void AjoutItems(List<Item> listeItem, int nbItemAjout, string[] listeCheminAjout ,int posXmin, int posXmax, int posYmin, int posYmax, int longCoteImageMin, int longCoteImageMax, bool rotation)
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
                Console.WriteLine("Add item, " + listeCheminAjout[indChemin] +" pos :"+posX+posY+longCoteImage);

                listeItem.Add( new Item(posX, posY, 90,30, listeCheminAjout[indChemin],longCoteImage,longCoteImage,carteBackground));

                if (rotation)
                {
                    listeItem[i].image.RenderTransform = new RotateTransform(rnd.Next(0, 361), longCoteImage, longCoteImage);
                }
            }



        }


        private void InitMinuterie()
        {
            CompositionTarget.Rendering += Jeu;
        }

        private void Jeu(object? sender, EventArgs e)
        {
            CheckDeplacement();

            if (canonActif)
                joueur.TirerBoulet();

            joueur.DeplacerBoulets();
        }

        private void CheckDeplacement()
        {
            double vitesse = joueur.caracteristique.vitesse;

            if  (deplacementDroite)
                DeplaceMonde(-vitesse, 0);
            if (deplacementGauche)
                DeplaceMonde(vitesse, 0);
            if (deplacementHaut)
                DeplaceMonde(0, vitesse);
            if (deplacementBas)
                DeplaceMonde(0, -vitesse);
        }

        private void InitBitmaps()
        {
            bitmapMer = new BitmapImage(new Uri(Chemin.IMAGE_MER));

            bitmapRocher1 = new BitmapImage(new Uri(Chemin.IMAGE_ROCHER1));
            bitmapRocher2 = new BitmapImage(new Uri(Chemin.IMAGE_ROCHER2));
            bitmapRocher3 = new BitmapImage(new Uri(Chemin.IMAGE_ROCHER3));
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

        

        private void LancerJeu()
        {
            hudJoueur.Visibility = Visibility.Visible;
            ile = new Ile(carteBackground);
            ile.ApparaitreIle();


            joueur = new Joueur(carteBackground);
            joueur.ApparaitreBateau();

            InitMinuterie();

            //Fonction de test des items.

            AjoutItems(listeItem, 70, [Chemin.IMAGE_TRESOR], 0, 4000, 0, 4000, 20, 70, false);
            AjoutItems(listeItem, 70, [Chemin.IMAGE_ROCHER1, Chemin.IMAGE_ROCHER2, Chemin.IMAGE_ROCHER3], 0, 4000, 0, 4000, 50, 200, true);
        }

        private void DeplaceMonde(double x, double y)
        {
            foreach (UIElement element in carteBackground.Children)
            {
                // Ignorer le joueur et autres éléments ne devant pas bouger
                if (element == joueur.bateau)
                    continue;

                if (x != 0)
                {
                    double positionX = Canvas.GetLeft(element);
                    Canvas.SetLeft(element, positionX + x);
                }

                if (y != 0)
                {
                    double positionY = Canvas.GetTop(element);
                    Canvas.SetTop(element, positionY + y);
                }
            }
        }

        private void Fenetre_KeyUp(object sender, KeyEventArgs e)
        {
            if (jouer)
            {
                if (e.Key == Key.D)
                {
                    deplacementDroite = false;
                }

                if (e.Key == Key.Q)
                {
                    deplacementGauche = false;
                }

                if (e.Key == Key.Z)
                {
                    deplacementHaut = false;
                }

                if (e.Key == Key.S)
                {
                    deplacementBas = false;
                }
            }
        }


        private void Fenetre_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
                canonActif = true;
        }

        private void Fenetre_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
                canonActif = false;
        }

        private void Fenetre_MouseMove(object sender, MouseEventArgs e)
        {

            if (jouer)
            {

                Point positionSouris = e.GetPosition(carteBackground);
                joueur.UpdateOrientation(positionSouris);
            }

        }

        private void Fenetre_KeyDown(object sender, KeyEventArgs e)
        {
            if (jouer)
            {
                if (e.Key == Key.D)
                {
                    deplacementDroite = true;
                }

                if (e.Key == Key.Q)
                {
                    deplacementGauche = true;
                }

                if (e.Key == Key.Z)
                {
                    deplacementHaut = true;
                }

                if (e.Key == Key.S)
                {
                    deplacementBas = true;
                }
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