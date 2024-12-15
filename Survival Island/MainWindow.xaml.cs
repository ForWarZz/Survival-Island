using System.Windows;
using System.Windows.Input;

namespace Survival_Island
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MoteurJeu moteurJeu;

        private bool jouer;
        private bool menuActif = false;

        public MainWindow()
        {
            InitializeComponent();

            moteurJeu = new MoteurJeu(this);
            jouer = false;
        }

        // Gestion des touches et de la souris
        private void Fenetre_KeyUp(object sender, KeyEventArgs e)
        {
            if (jouer && e.Key == Key.Z)
            {

                moteurJeu.Joueur.deplacement = false;
            }
        }

        private void Fenetre_KeyDown(object sender, KeyEventArgs e)
        {
            if (jouer && e.Key == Key.Z)
                moteurJeu.Joueur.deplacement = true;

            if (jouer && e.Key == Key.G)
            {
                Console.WriteLine("God mod");
                moteurJeu.Joueur.ModeTriche = true;
            }
        }

        private void Fenetre_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
                moteurJeu.Joueur.canonActif = true;
        }

        private void Fenetre_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
                moteurJeu.Joueur.canonActif &= false;
        }

        private void Fenetre_MouseMove(object sender, MouseEventArgs e)
        {
            if (jouer)
                moteurJeu.Joueur.ChangerOrientation(e.GetPosition(carte));
        }

        // Gestions du menu d'amélioration
        private void btnAmeliorations_Click(object sender, RoutedEventArgs e)
        {
            if (menuActif)
            {
                menuActif = false;
                spAmelio.Visibility = Visibility.Hidden;
                btnAmeliorations.Content = "Améliorations";

                carte.Focus();
            }
            else
            {
                menuActif = true;
                spAmelio.Visibility = Visibility.Visible;
                btnAmeliorations.Content = "Fermer";

                spAmelio.Focus();
            }
        }

        // Menu accueil
        
        private void btnJouer_Click(object sender, RoutedEventArgs e)
        {
            menuAccueil.Visibility = Visibility.Hidden;
            jouer = true;
            moteurJeu.InitJeu();
        }

        private void btnFermerJeu_Click(object sender, RoutedEventArgs e)
        {
            FermerJeu();
        }

        private void MenuQuitter_Click(object sender, RoutedEventArgs e)
        {
            FermerJeu();
        }

        private void FermerJeu()
        {
            MessageBoxResult result = MessageBox.Show("Êtes vous sûr de vouloir quitter le jeu ?", "Fermer le jeu", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
                Application.Current.Shutdown();
        }

        private void MenuSon_Click(object sender, RoutedEventArgs e)
        {
            DialogueAudio dialog = new DialogueAudio();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                double VolumeMusique = dialog.SlideMusique.Value;
                double VolumeSon = dialog.SlideSon.Value;
            }
        }

        private void MenuChangerBateau_Click(object sender, RoutedEventArgs e)
        {
            DialogueChangerBateau dialog = new DialogueChangerBateau();
            bool? result = dialog.ShowDialog();
            /// faire système pour changer le skin

        }

        private void btnVieBateauAmelio_Click(object sender, RoutedEventArgs e)
        {
            moteurJeu.Joueur.AmelioVie();
        }

        private void btnDegatsBateauAmelio_Click(object sender, RoutedEventArgs e)
        {
            moteurJeu.Joueur.AmelioDegats();
        }

        private void btnVitesseBateauAmelio_Click(object sender, RoutedEventArgs e)
        {
            moteurJeu.Joueur.AmelioVitesse();
        }

        private void btnVieIleAmelio_Click(object sender, RoutedEventArgs e)
        {
            moteurJeu.Ile.AmelioVie();
        }
    }
}
