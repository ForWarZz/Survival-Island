using Survival_Island.Entites;
using Survival_Island.Outils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Survival_Island
{
    /// <summary>
    /// Logique d'interaction pour DialogueChangerBateau.xaml
    /// </summary>
    public partial class DialogueChangerBateau : Window
    {
        public int numBateau;

        private BitmapImage[] images;
        public BitmapImage imageActuelle;

        private Joueur joueur;

        public DialogueChangerBateau(MoteurJeu moteurJeu)
        {
            InitializeComponent();

            joueur = moteurJeu.Joueur;
            images = joueur.Images;
            imageActuelle = (BitmapImage)((Image)joueur.CanvaElement).Source;
            imgBateau.Source = imageActuelle;
            numBateau = moteurJeu.NumBateau;

        }
        
        private void ChangerBateau()
        {
            imageActuelle = images[numBateau];
            imgBateau.Source = imageActuelle;
            ((Image)joueur.CanvaElement).Source = imageActuelle;
        }

        private void btnDroite_Click(object sender, RoutedEventArgs e)
        {
            numBateau = (numBateau+1)%images.Length;
            ChangerBateau();
        }

        private void btnGauche_Click(object sender, RoutedEventArgs e)
        {
            /// Selon C#, (-1)%4 = -1 et pas 3 ...
            if (numBateau == 0)
                numBateau = images.Length+((numBateau - 1) % images.Length);
            else
                numBateau = (numBateau - 1) % images.Length;
            ChangerBateau();
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
