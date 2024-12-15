using Survival_Island.Entites.Base;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Survival_Island.Entites.Objets
{
    public class ObjetRecompense : EntiteAvecVie
    {
        public int valeurRecompense { get; set; }
        public TypeRecompense type { get; private set; }

        public ProgressBar vieBar { get; set; }

        public ObjetRecompense(Canvas carte, BitmapImage bitmapImage, int largeur, int hauteur, int valeurRecompense, TypeRecompense type, int vieMax, bool statique) : base(carte, statique, true, vieMax)
        {
            this.carte = carte;
            this.valeurRecompense = valeurRecompense;
            this.type = type;

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
