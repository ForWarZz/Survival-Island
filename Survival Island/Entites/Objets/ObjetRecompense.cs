using Survival_Island.Entites.Base;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Survival_Island.Entites.Objets
{
    public class ObjetRecompense : EntiteAvecVie
    {
        public int ValeurRecompense { get; }
        public TypeRecompense Type { get; }

        public ObjetRecompense(Canvas carte, BitmapImage bitmapImage, int largeur, int hauteur, int valeurRecompense, TypeRecompense type, int vieMax, bool statique) : base(carte, statique, true, vieMax)
        {
            ValeurRecompense = valeurRecompense;
            Type = type;

            CanvaElement = new Image();
            ((Image)CanvaElement).Source = bitmapImage;
            CanvaElement.Width = largeur;
            CanvaElement.Height = hauteur;
        }

        public ObjetRecompense(Canvas carte, BitmapImage bitmapImage, int vie, int valeurRecompense, TypeRecompense type, bool statique) :
            this(carte, bitmapImage, (int)bitmapImage.Width, (int)bitmapImage.Height, valeurRecompense, type, vie, statique)
        { }
    }
}
