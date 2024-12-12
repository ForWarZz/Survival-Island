using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Survival_Island
{
    internal class Item
    {

        public Image image { get; set; }
        public string cheminImage { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public double vie { get; set; } = 90;
        public int experience { get; set; } = 30;
        public int largeur { get; set; }
        public int longeur { get; set; }
        public Canvas canvasToAdd { get; set; }

        public Item(int positionX, int positionY, int vie ,int experience, string cheminImage, int largeur, int longeur, Canvas canvasToAdd)
        {
            this.image = new Image();
            this.positionX = positionX;
            this.positionY = positionX;
            this.vie = vie;
            this.experience = experience;
            this.image.Source = new BitmapImage(new Uri(cheminImage));
            this.image.Width = largeur;
            this.image.Height = longeur;

            Canvas.SetLeft(this.image, positionX);
            Canvas.SetTop(this.image, positionY);

            this.canvasToAdd = canvasToAdd;
            canvasToAdd.Children.Add(this.image);


        }

        public void setPosition(int nouveauX, int nouveauY)
        {

            Canvas.SetLeft(this.image, positionX);
            Canvas.SetTop(this.image, positionY);
        }

        public double prendDesDegats(double degats)
        {
            this.vie -= degats;
            if (this.vie <= 0)
            {
                this.canvasToAdd.Children.Remove(this.image);
            }
            return vie;
        }
    }
}
