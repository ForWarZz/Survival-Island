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
            this.carte = carte;
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
            double centerX = carte.Width / 2 - CanvaElement.Width / 2;
            double centerY = carte.Height / 2 - CanvaElement.Height / 2;

            Apparaitre(centerX, centerY);
        }

        public override bool InfligerDegats(int degats)
        {
            bool detruit = base.InfligerDegats(degats);

            if (vie <= vieMax / 2)
                ((Image)CanvaElement).Source = bitmapIleFaible;
            ActualiserHUD();

            return detruit;
        }

        public void ActualiserHUD()
        {
            fenetre.barreVieIle.Value = vie;
            fenetre.barreVieIle.Maximum = vieMax;

            fenetre.txtVieIle.Text = vie + "/" + vieMax + " PV";

            ActualiserMenuAmelioration();
        }
        public void ActualiserMenuAmelioration()
        {
            fenetre.txtVieIleAmelio.Text = vie.ToString();
        }

        public void AmelioVie()
        {
            if (moteurJeu.Joueur.PointsAmeliorations > 0)
            {
                vieMax += Constante.AMELIO_VIE_ILE;
                vie += Constante.AMELIO_VIE_ILE;

                moteurJeu.Joueur.PointsAmeliorations--;
                moteurJeu.Joueur.ActualiserHUD();

                ActualiserHUD();
            }
        }
    }
}
