using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Survival_Island.carte
{
    internal class Objet : Collision
    {
        public int vie { get; set; }
        public int experience { get; set; }

        public Objet(Canvas carte, BitmapImage bitmapImage, int largeur, int hauteur, int vie, int experience) : base(carte)
        {
            this.carte = carte;
            this.vie = vie;
            this.experience = experience;

            element = new Image();
            ((Image)element).Source = bitmapImage;
            element.Width = largeur;
            element.Height = hauteur;
        }

        public Objet(Canvas carte, BitmapImage bitmapImage, int vie, int experience) :
            this(carte, bitmapImage, (int)bitmapImage.Width, (int)bitmapImage.Height, vie, experience)
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
