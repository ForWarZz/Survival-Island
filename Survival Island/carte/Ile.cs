using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Survival_Island.Outils;
using Survival_Island.Outils.Entites;

namespace Survival_Island.carte
{
    public class Ile: EntiteBase
    {
        private MainWindow fenetre;

        private BitmapImage bitmapIle, bitmapIleFaible;

        public int vieMax { get; set; } = Constante.ILE_VIE_MAX;
        public int vie { get; private set; } = Constante.ILE_VIE_MAX;

        public Ile(Canvas carte, MainWindow fenetre): base(carte, true)
        {
            this.carte = carte;
            this.fenetre = fenetre;

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

        public bool InfligerDegats(int degats)
        {
            vie -= degats;
            Image image = (Image)canvaElement;

            if (vie <= vieMax / 2 && image.Source != bitmapIleFaible)
                image.Source = bitmapIleFaible;

            if (vie <= 0)
            {
                return true;
            }

             ActualiserHUD();

            return false;
        }

        public void ActualiserHUD()
        {
            fenetre.barreVieIle.Value = vie;
            fenetre.barreVieIle.Maximum = vieMax;

            fenetre.txtVieIle.Text = vie + "/" + vieMax + " PV";
        }
    }
}
