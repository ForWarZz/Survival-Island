using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Survival_Island.ile
{
    internal class Ile
    {
        private Canvas carte;
        private BitmapImage bitmapIle, bitmapIleFaible;

        public Image image;
        public int vieMax = 1000;
        public int vie = 1000;

        public Ile(Canvas carte)
        {
            this.carte = carte;
            InitBitmaps();

            image = new Image();
            image.Source = bitmapIle;
            image.Width = bitmapIle.PixelWidth;
            image.Height = bitmapIle.PixelHeight;
        }

        private void InitBitmaps()
        {
            bitmapIle = new BitmapImage(new Uri(Chemin.IMAGE_ILE));
            bitmapIleFaible = new BitmapImage(new Uri(Chemin.IMAGE_ILEC));
        }

        public void ApparaitreIle()
        {
            double centerX = carte.Width / 2 - image.Width / 2;
            double centerY = carte.Height / 2 - image.Height / 2;

            Canvas.SetLeft(image, centerX);
            Canvas.SetTop(image, centerY);

            carte.Children.Add(image);
        }

        public Rectangle RecupereCollisionRectangle()
        {
            double ileX = Canvas.GetLeft(image);
            double ileY = Canvas.GetTop(image);

            return new Rectangle((int) ileX, (int) ileY, (int) image.Width, (int) image.Height);
        }

        public void InfligerDegats(int degats)
        {
            vie -= degats;
            CheckVie();
        }

        private void CheckVie()
        {
            // Verifier sa vie, changer l'image en fonction....
        }
    }
}
