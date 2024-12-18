using Survival_Island.Outils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Survival_Island
{
    public class GestionImages
    {
        public BitmapImage Mer { get; private set; }
        public BitmapImage[] Rochers { get; private set; }
        public BitmapImage Tresor { get; private set; }
        public BitmapImage[] BateauxJoueur { get; private set; }
        public BitmapImage BateauEnnemi { get; private set; }
        public BitmapImage Ile { get; private set; }
        public BitmapImage IleFaible { get; private set; }

        public GestionImages()
        {
            Mer = ChargerImage(Chemin.IMAGE_MER);
            Tresor = ChargerImage(Chemin.IMAGE_TRESOR);
            Rochers = ChargerImages(Chemin.ROCHERS, Constante.NOMBRE_IMG_ROCHERS, Constante.EXT_IMG_ROCHERS);

            BateauxJoueur = ChargerImages(Chemin.BATEAUX_JOUEUR, Constante.NOMBRE_IMG_BATEAU_JOUEUR, Constante.EXT_IMG_BATEAU_JOUEUR);

            BateauEnnemi = ChargerImage(Chemin.IMAGE_BATEAU_ENNEMI);

            Ile = ChargerImage(Chemin.IMAGE_ILE);
            IleFaible = ChargerImage(Chemin.IMAGE_ILEC);
        }

        private BitmapImage ChargerImage(string chemin)
        {
            return new BitmapImage(new Uri(chemin));
        }

        private BitmapImage[] ChargerImages(string chemin, int nombreImages, string extension)
        {
            BitmapImage[] images = new BitmapImage[nombreImages];
            for (int i = 1; i <= nombreImages; i++)
            {
                images[i - 1] = ChargerImage(chemin + i + "." + extension);
            }

            return images;
        }
    }
}
