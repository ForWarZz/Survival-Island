using Survival_Island.Entites;
using Survival_Island.Outils;
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
        private bool menuActif = false;
        private bool activePause = false;
        private bool jeu = false;


        public MediaPlayer mediaPlayerMusique = new MediaPlayer();

        public MediaPlayer mediaPlayerSon = new MediaPlayer();



        public MainWindow()
        {
            InitializeComponent();

            moteurJeu = new MoteurJeu(this);
            jouer = false;

        }

        
        
        private void Pause()
        {
            // Quand le menu est en pause, cacher le hudjoueur et le bouton menu améliorations
            if (jouer)
            {
                hudJoueur.Visibility = Visibility.Visible;
                btnAmeliorations.Visibility = Visibility.Visible;
            }
            else
            {
                hudJoueur.Visibility = Visibility.Hidden;
                btnAmeliorations.Visibility = Visibility.Hidden;
            }
        }

        // Gestion des touches et de la souris
        private void Fenetre_KeyUp(object sender, KeyEventArgs e)
        {
            if (jouer && e.Key == Key.Z)
            {

                moteurJeu.Joueur.Deplacement = false;
            }
        }

        private void Fenetre_KeyDown(object sender, KeyEventArgs e)
        {
            if (jouer && e.Key == Key.Z)
                moteurJeu.Joueur.Deplacement = true;

            if (jouer && e.Key == Key.G)
            {
                Console.WriteLine("God mod");
                moteurJeu.Joueur.ModeTriche = true;
            }
            if (jouer && e.Key == Key.Escape)
            {

                menuAccueil.Visibility = Visibility.Visible;
                menuAccueil.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#50000000");
                btnJouer.Content = "Retour au jeu";

                jouer = false ;
                Pause();
                moteurJeu.Joueur.Deplacement = false;
            }
        }

        private void Fenetre_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
            {
                moteurJeu.Joueur.CanonActif = true;

                
            }
        }

        private void Fenetre_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
                moteurJeu.Joueur.CanonActif = false;
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
            jeu = true;
            if (!activePause)
            {
                moteurJeu.InitJeu();
                activePause = true;
                txtPause.Text = "PAUSE";
            }
            Pause();
        }

        private void btnFermerJeu_Click(object sender, RoutedEventArgs e)
        {
            jeu = false;
            FermerJeu();
        }

        private void MenuQuitter_Click(object sender, RoutedEventArgs e)
        {
            jeu = false;
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
            DialogueAudio dialog = new DialogueAudio(moteurJeu, jouer);

            bool? result = dialog.ShowDialog();

            if (result == false)
            {
                moteurJeu.mediaPlayerMusique.Volume = dialog.SlideMusique.Value;
                if (jouer)
                {
                    Console.WriteLine("Ca joue et sa console");
                    //moteurJeu.Joueur.soundPlayerTire.Volume = dialog.SlideSon.Value;
                }
            }
        }

        private void MenuBateauChange()
        {
            if (jeu)
            {
                DialogueChangerBateau dialog = new DialogueChangerBateau(moteurJeu);
                bool? result = dialog.ShowDialog();
                moteurJeu.numBateau = dialog.numBateau;
            }
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
            moteurJeu.Ile.AmelioVie();
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
    }
}
