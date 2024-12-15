using Survival_Island.Entites.Base;
using Survival_Island.Outils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Survival_Island.Entites.Navire
{
    public class Boulet : EntiteBase
    {
        public Vector Direction { get; private set; }
        public Bateau Tireur { get; private set; }

        public Boulet(Canvas carte, Vector direction, Bateau tireur) : base(carte, false)
        {
            Ellipse bouletEllipse = new Ellipse();
            bouletEllipse.Width = Constante.TAILLE_BOULET;
            bouletEllipse.Height = Constante.TAILLE_BOULET;
            bouletEllipse.Fill = Brushes.Black;

            CanvaElement = bouletEllipse;

            Direction = direction;
            Tireur = tireur;
        }
    }
}
