using Survival_Island.Outils;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Survival_Island
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MoteurJeu moteurJeu;

        private bool jouer;
        private bool pauseActive;
        public bool menuActif = false;

        public MainWindow()
        {
            InitializeComponent();

            moteurJeu = new MoteurJeu(this);

            jouer = false;
            pauseActive = false;
        }

        // Gestion des touches et de la souris
        private void Fenetre_KeyUp(object sender, KeyEventArgs e)
        {
            if (jouer && e.Key == Key.Z)
            {
                //Console.WriteLine("DEBUG: Z relaché");
                moteurJeu.Joueur.Deplacement = false;
            }
        }

        private void Fenetre_KeyDown(object sender, KeyEventArgs e)
        {
            if (jouer && e.Key == Key.Z)
            {
                //Console.WriteLine("DEBUG: Z pressé");
                moteurJeu.Joueur.Deplacement = true;
            }

            if (jouer && e.Key == Key.G)
            {
                //Console.WriteLine("DEBUG: Mode triche activé");
                moteurJeu.Joueur.ModeTriche = true;
            }

            if (jouer && e.Key == Key.Escape)
            {
                //Console.WriteLine("DEBUG: Menu pause");

                menuAccueil.Visibility = Visibility.Visible;
                txtPause.Visibility = Visibility.Visible;

                menuAccueil.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#50000000");
                btnJouer.Content = "Retour au jeu";

                jouer = false;
                pauseActive = true;
                moteurJeu.Pause();
            }
        }

        private void Fenetre_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
            {
                //Console.WriteLine("DEBUG: Activer canon joueur");
                moteurJeu.Joueur.CanonActif = true;
            }
        }

        private void Fenetre_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
            {
                //Console.WriteLine("DEBUG: Désactiver canon joueur");
                moteurJeu.Joueur.CanonActif = false;
            }
        }

        private void Fenetre_MouseMove(object sender, MouseEventArgs e)
        {
            if (jouer)
                moteurJeu.Joueur.ChangerOrientation(e.GetPosition(carte));
        }

        // Gestions du menu d'amélioration
        private void btnAmeliorations_Click(object sender, RoutedEventArgs e)
        {
            moteurJeu.Joueur.NouveauNiveau = false;

            if (menuActif)
            {
                menuActif = false;
                spAmelio.Visibility = Visibility.Hidden;
                btnAmeliorations.Content = "Améliorations";
            }
            else
            {
                menuActif = true;
                spAmelio.Visibility = Visibility.Visible;
                btnAmeliorations.Content = "Fermer";
            }
        }

        // Menu accueil

        private void btnJouer_Click(object sender, RoutedEventArgs e)
        {
            menuAccueil.Visibility = Visibility.Hidden;

            if (pauseActive)
            {
                menuAccueil.Visibility = Visibility.Hidden;
                txtPause.Visibility = Visibility.Hidden;

                moteurJeu.Pause();
                pauseActive = false;
            }
            else
            {
                moteurJeu.InitJeu();
            }

            jouer = true;
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

        private void MenuSonAffiche()
        {
            DialogueAudio dialog = new DialogueAudio(moteurJeu);
            bool? result = dialog.ShowDialog();

            if (result == false)
            {
                moteurJeu.GestionSons.MediaPlayerMusique.Volume = dialog.SlideMusique.Value;
            }
        }

        private void MenuBateauChange()
        {
            DialogueChangerBateau dialog = new DialogueChangerBateau(moteurJeu);
            dialog.ShowDialog();

            moteurJeu.NumBateau = dialog.numBateau;
        }

        private void MenuSonClick(object sender, RoutedEventArgs e)
        {
            MenuSonAffiche();
        }

        private void MenuChangerBateau_Click(object sender, RoutedEventArgs e)
        {
            MenuBateauChange();
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
            if (moteurJeu.Joueur.VitesseMax >= Constante.AMELIO_VITESSE_MAX)
            {
                btnVitesseBateauAmelio.Visibility = Visibility.Hidden;
            }
        }

        private void btnVieIleAmelio_Click(object sender, RoutedEventArgs e)
        {
            moteurJeu.GestionCarte.Ile.AmelioVie();
        }

        private void Fenetre_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
                moteurJeu.Joueur.Deplacement = true;
        }

        private void Fenetre_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
                moteurJeu.Joueur.Deplacement = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            spParametres.Visibility = Visibility.Hidden;
            menuAccueil.Visibility = Visibility.Visible;
        }

        private void btnOuvrirOptions_Click(object sender, RoutedEventArgs e)
        {
            menuAccueil.Visibility = Visibility.Hidden;
            spParametres.Visibility = Visibility.Visible;
        }

        private void btnAudio_Click(object sender, RoutedEventArgs e)
        {
            MenuSonAffiche();
        }

        private void btnChangeBateau_Click(object sender, RoutedEventArgs e)
        {
            MenuBateauChange();
        }

        private void btnRejouer_Click(object sender, RoutedEventArgs e)
        {
            spMenuFin.Visibility = Visibility.Hidden;
            moteurJeu.Rejouer();
        }
    }
}
