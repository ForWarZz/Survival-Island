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
        private Joueur joueur;



        public DialogueAudio(MoteurJeu moteurJeu, bool jeuLance)
        {
            InitializeComponent();

            this.SlideMusique.Value = moteurJeu.MediaPlayerMusique.Volume;

            if (jeuLance)
            {
                //this.SlideSon.Value = moteurJeu.Joueur.mediaPlayerSon.Volume;

            }


            



        }
    }
}
