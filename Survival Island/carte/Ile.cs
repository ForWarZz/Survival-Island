using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Survival_Island.Outils;
using Survival_Island.Outils.Entites;

namespace Survival_Island.carte
{
    public class Ile: EntiteAvecVie
    {
        private MainWindow fenetre;

        private BitmapImage bitmapIle, bitmapIleFaible;

        private MoteurJeu moteurJeu;

        public Ile(Canvas carte, MainWindow fenetre, MoteurJeu moteurJeu): base(carte, true, false, Constante.ILE_VIE_MAX)
        {
            this.carte = carte;
            this.fenetre = fenetre;
            this.moteurJeu = moteurJeu;

            InitBitmaps();

            canvaElement = new Image();
            ((Image) canvaElement).Source = bitmapIle;
            canvaElement.Width = bitmapIle.PixelWidth;
            canvaElement.Height = bitmapIle.PixelHeight;

            ActualiserHUD();
        }

        private void InitBitmaps()
        {
            bitmapIle = new BitmapImage(new Uri(Chemin.IMAGE_ILE));
            bitmapIleFaible = new BitmapImage(new Uri(Chemin.IMAGE_ILEC));
        }

        public void Apparaitre()
        {
            double centerX = carte.Width / 2 - canvaElement.Width / 2;
            double centerY = carte.Height / 2 - canvaElement.Height / 2;

            Apparaitre(centerX, centerY);
        }

        public override bool InfligerDegats(int degats)
        {
            bool detruit = base.InfligerDegats(degats);
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
            if (moteurJeu.joueur.pointsAmeliorations > 0)
            {
                vieMax += Constante.AMELIO_VIE_ILE;
                vie += Constante.AMELIO_VIE_ILE;

                moteurJeu.joueur.pointsAmeliorations--;
                moteurJeu.joueur.ActualiserHUD();

                ActualiserHUD();
            }
        }
    }
}
