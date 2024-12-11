using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Survival_Island.carte
{
    internal class Ile: Collision
    {
        private ProgressBar progressBar;
        private TextBlock valeurProgressBar;

        private BitmapImage bitmapIle, bitmapIleFaible;

        public int vieMax = Constante.ILE_VIE_MAX;
        public int vie = Constante.ILE_VIE_MAX;

        public Ile(Canvas carte, ProgressBar progressBar, TextBlock valeurProgressBar): base(carte)
        {
            this.carte = carte;
            this.progressBar = progressBar;
            this.valeurProgressBar = valeurProgressBar;

            InitBitmaps();

            element = new Image();
            ((Image) element).Source = bitmapIle;
            element.Width = bitmapIle.PixelWidth;
            element.Height = bitmapIle.PixelHeight;

            UpdateHUD();
        }

        private void InitBitmaps()
        {
            bitmapIle = new BitmapImage(new Uri(Chemin.IMAGE_ILE));
            bitmapIleFaible = new BitmapImage(new Uri(Chemin.IMAGE_ILEC));
        }

        public override void Apparaitre()
        {
            double centerX = carte.Width / 2 - element.Width / 2;
            double centerY = carte.Height / 2 - element.Height / 2;

            Canvas.SetLeft(element, centerX);
            Canvas.SetTop(element, centerY);

            carte.Children.Add(element);
        }

        public void InfligerDegats(int degats)
        {
            vie -= degats;
            Image image = (Image)element;

            if (vie <= vieMax / 2 && image.Source != bitmapIleFaible)
                image.Source = bitmapIleFaible;

            UpdateHUD();
        }

        public void UpdateHUD()
        {
            progressBar.Value = vie * 100 / vieMax;
            valeurProgressBar.Text = vie + "/" + vieMax + " PV";
        }
    }
}
