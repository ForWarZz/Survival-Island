using Survival_Island.Entites;
using System.Windows;

namespace Survival_Island
{
    /// <summary>
    /// Logique d'interaction pour DialogueAudio.xaml
    /// </summary>
    /// 
    

    public partial class DialogueAudio : Window
    {
        MoteurJeu moteurJeu;
        public DialogueAudio(MoteurJeu moteurJeu)
        {
            InitializeComponent();
            this.moteurJeu = moteurJeu;
            SlideMusique.Value = moteurJeu.GestionSons.MediaPlayerMusique.Volume;
        }

        private void btnAppliquer_Click(object sender, RoutedEventArgs e)
        {
            moteurJeu.GestionSons.MediaPlayerMusique.Volume = SlideMusique.Value;
            DialogResult = true;
        }
    }
}
