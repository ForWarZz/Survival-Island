using Survival_Island.Entites.Base;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Survival_Island.Entites.Objets
{
    public class ObjetRecompense : EntiteAvecVie
    {
        public int RecompenseExperience { get; }
        public int RecompenseVie { get;  }

        public ObjetRecompense(Canvas carte, BitmapImage bitmapImage, int largeur, int hauteur, int experience, int vie, int vieMaxObjet) : base(carte, true, true, vieMaxObjet)
        {
            RecompenseExperience = experience;
            RecompenseVie = vie;

            CanvaElement = new Image();
            ((Image)CanvaElement).Source = bitmapImage;
            CanvaElement.Width = largeur;
            CanvaElement.Height = hauteur;
        }

        public ObjetRecompense(Canvas carte, BitmapImage bitmapImage, int vieMaxObjet, int experience, int vie) :
            this(carte, bitmapImage, (int)bitmapImage.Width, (int)bitmapImage.Height, experience, vie, vieMaxObjet)
        { }
    }
}
