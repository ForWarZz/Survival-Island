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
        public DialogueAudio(MoteurJeu moteurJeu)
        {
            InitializeComponent();
            SlideMusique.Value = moteurJeu.GestionSons.MediaPlayerMusique.Volume;
        }
    }
}
