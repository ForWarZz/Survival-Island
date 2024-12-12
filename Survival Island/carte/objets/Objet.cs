using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Survival_Island.carte.objets
{
    internal class Objet : Collision
    {
        public int vie { get; private set; }
        public int experience { get; set; }

        public Objet(Canvas carte, BitmapImage bitmapImage, int largeur, int hauteur, int vie, int experience, bool statique) : base(carte, statique)
        {
            this.carte = carte;
            this.vie = vie;
            this.experience = experience;

            canvaElement = new Image();
            ((Image)canvaElement).Source = bitmapImage;
            canvaElement.Width = largeur;
            canvaElement.Height = hauteur;
        }

        public Objet(Canvas carte, BitmapImage bitmapImage, int vie, int experience, bool statique) :
            this(carte, bitmapImage, (int)bitmapImage.Width, (int)bitmapImage.Height, vie, experience, statique)
        { }

        public bool InfligerDegats(int degats)
        {
            vie -= degats;

            if (vie <= 0)
            {
                Disparaitre();
                return true;
            }

            return false;
        }
    }
}
