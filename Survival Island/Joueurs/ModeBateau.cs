using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Survival_Island.Joueurs
{
    public abstract class ModeBateau
    {
        public abstract void Tirer(Canvas carte, double posX, double posY, List<Boulet> boulets, Vector orientation, Joueur joueur);
    }

    public class ModeClassic : ModeBateau
    {
        public override void Tirer(Canvas carte, double posX, double posY, List<Boulet> boulets, Vector orientation, Joueur joueur)
        {
            Boulet boulet = new Boulet(carte, orientation, joueur);
            boulet.Apparaitre(posX, posY);
            boulets.Add(boulet);
        }
    }

    public class ModeDouble : ModeBateau
    {
        public override void Tirer(Canvas carte, double posX, double posY, List<Boulet> boulets, Vector orientation, Joueur joueur)
        {
            Boulet boulet1 = new Boulet(carte, orientation, joueur);
            boulet1.Apparaitre(posX - 8, posY);
            boulets.Add(boulet1);

            Boulet boulet2 = new Boulet(carte, orientation, joueur);
            boulet2.Apparaitre(posX + 8, posY);
            boulets.Add(boulet2);
        }
    }
}
