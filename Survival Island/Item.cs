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
using System.Windows.Media.Media3D;
using System.Windows.Threading;
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

        public ProgressBar pBar {get; set;}

        public DispatcherTimer minuterieItemSeconde {get; set;}

        public int tempsAffichepBar;

        public Item(int positionX, int positionY, int vie ,int experience, string cheminImage, int largeur, int longeur, Canvas canvasToAdd)
        {
            

            this.tempsAffichepBar = 0;
            
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

            // Code de visualisation de vie
            this.pBar = new ProgressBar();
            Canvas.SetLeft(this.pBar, positionX +this.image.Height / 4);
            Canvas.SetTop(this.pBar, positionY + this.image.Width);
            canvasToAdd.Children.Add(this.pBar);
            this.pBar.Width = this.image.Width / 2;
            this.pBar.Height = 10;
            this.pBar.Minimum = 0;
            this.pBar.Maximum = vie;
            this.pBar.Value = vie;
            InitialiserminuterieItemSeconde();
            this.pBar.Visibility = Visibility.Hidden;


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
                this.canvasToAdd.Children.Remove(this.pBar);
                this.canvasToAdd.Children.Remove(this.image);
            }
            this.pBar.Visibility = Visibility.Visible;
            this.pBar.Value = vie;
            this.tempsAffichepBar = 0;
            return vie;

        }

        private void InitialiserminuterieItemSeconde()
        {
            this.minuterieItemSeconde = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            this.minuterieItemSeconde.Tick += boucleTempsItem;
            
            this.minuterieItemSeconde.Start();
        }

        private void boucleTempsItem(object? sender, EventArgs e)
        {
            this.tempsAffichepBar += 1;
        }
    }
}
