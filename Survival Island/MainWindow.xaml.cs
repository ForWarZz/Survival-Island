﻿using Survival_Island.Outils;
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

        public MainWindow()
        {
            InitializeComponent();

            moteurJeu = new MoteurJeu(this);
            jouer = false;
        }

        private void Pause()
        {
            // Quand le menu est en pause, il faut cacher le hudjoueur et le bouton menu améliorations
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
                moteurJeu.Joueur.CanonActif = true;
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
            if (!activePause)
            {
                moteurJeu.InitJeu();
                activePause = true;
            }
            Pause();
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
            DialogueChangerBateau dialog = new DialogueChangerBateau(moteurJeu);
            bool? result = dialog.ShowDialog();
            moteurJeu.numBateau = dialog.numBateau;
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
    }
}
