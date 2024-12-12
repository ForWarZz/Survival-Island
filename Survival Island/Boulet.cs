using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Survival_Island.carte;

namespace Survival_Island
{
    internal class Boulet: Collision
    {
        public Vector direction { get; set; }

        public Boulet(Canvas carte, Vector direction): base(carte, false)
        {
            Ellipse bouletEllipse = new Ellipse();
            bouletEllipse.Width = Constante.TAILLE_BOULET;
            bouletEllipse.Height = Constante.TAILLE_BOULET;
            bouletEllipse.Fill = Brushes.Black;

            canvaElement = bouletEllipse;
            this.direction = direction;
        }
    }
}
