using Survival_Island.Entites.Base;
using Survival_Island.Outils;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Survival_Island.Entites
{
    public class Ile : EntiteAvecVie
    {
        private MainWindow fenetre;

        private BitmapImage bitmapIle, bitmapIleFaible;

        private MoteurJeu moteurJeu;

        public Ile(Canvas carte, MainWindow fenetre, MoteurJeu moteurJeu) : base(carte, true, false, Constante.ILE_VIE_MAX)
        {
            this.fenetre = fenetre;
            this.moteurJeu = moteurJeu;

            InitBitmaps();

            CanvaElement = new Image();
            ((Image)CanvaElement).Source = bitmapIle;
            CanvaElement.Width = bitmapIle.PixelWidth;
            CanvaElement.Height = bitmapIle.PixelHeight;

            ActualiserHUD();
        }

        private void InitBitmaps()
        {
            bitmapIle = new BitmapImage(new Uri(Chemin.IMAGE_ILE));
            bitmapIleFaible = new BitmapImage(new Uri(Chemin.IMAGE_ILEC));
        }

        public void Apparaitre()
        {
            double centerX = Carte.Width / 2 - CanvaElement.Width / 2;
            double centerY = Carte.Height / 2 - CanvaElement.Height / 2;

            Apparaitre(centerX, centerY);
        }

        public override bool InfligerDegats(int degats)
        {
            bool detruit = base.InfligerDegats(degats);

            if (Vie <= VieMax / 2)
                ((Image)CanvaElement).Source = bitmapIleFaible;

            return detruit;
        }

        private void ActualiserHUD()
        {
            fenetre.barreVieIle.Value = Vie;
            fenetre.barreVieIle.Maximum = VieMax;

            fenetre.txtVieIle.Text = Vie + "/" + VieMax + " PV";

            ActualiserMenuAmelioration();
        }

        private void ActualiserMenuAmelioration()
        {
            fenetre.txtVieIleAmelio.Text = Vie.ToString();
        }

        public override void MettreAJour()
        {
            base.MettreAJour();
            ActualiserHUD();
        }

        public void AmelioVie()
        {
            if (moteurJeu.Joueur.PointsAmeliorations > 0)
            {
                VieMax += Constante.AMELIO_VIE_ILE;
                Vie += Constante.AMELIO_VIE_ILE;

                moteurJeu.Joueur.PointsAmeliorations--;
                moteurJeu.Joueur.MettreAJour();

                ActualiserHUD();
            }
        }
    }
}
