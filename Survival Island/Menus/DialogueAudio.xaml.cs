using Survival_Island.Outils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
            SlideMusique.Value = moteurJeu.GestionSons.Musiques[moteurJeu.GestionSons.IndiceMusiqueJoue].Volume;
        }

        private void btnAppliquer_Click(object sender, RoutedEventArgs e)
        {
            moteurJeu.GestionSons.Musiques[moteurJeu.GestionSons.IndiceMusiqueJoue].Volume = SlideMusique.Value;
            DialogResult = true;
        }

        private void btnMusique_Click(object sender, RoutedEventArgs e)
        {
            this.moteurJeu.GestionSons.MusiqueSuivante();
        }

        private void btnSon_Click(object sender, RoutedEventArgs e)
        {
            this.moteurJeu.GestionSons.SonSuivant();
        }
    }
}
