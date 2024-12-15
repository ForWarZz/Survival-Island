using System.Windows.Controls;

namespace Survival_Island.Entites.Navire
{
    public abstract class ModeBateau
    {
        protected Canvas carte;
        protected List<Boulet> boulets;
        protected Bateau tireur;

        protected ModeBateau(Canvas carte, List<Boulet> boulets, Bateau tireur)
        {
            this.carte = carte;
            this.boulets = boulets;
            this.tireur = tireur;
        }

        public abstract void Tirer(double posX, double posY);
    }

    public class ModeClassic : ModeBateau
    {
        public ModeClassic(Canvas carte, List<Boulet> boulets, Bateau tireur) : base(carte, boulets, tireur)
        { }

        public override void Tirer(double posX, double posY)
        {
            Boulet boulet = new Boulet(carte, tireur.orientation, tireur);
            boulet.Apparaitre(posX, posY);
            boulets.Add(boulet);
        }
    }

    public class ModeDouble : ModeBateau
    {
        public ModeDouble(Canvas carte, List<Boulet> boulets, Bateau tireur) : base(carte, boulets, tireur)
        { }

        public override void Tirer(double posX, double posY)
        {
            Boulet boulet1 = new Boulet(carte, tireur.orientation, tireur);
            boulet1.Apparaitre(posX - 8, posY);
            boulets.Add(boulet1);

            Boulet boulet2 = new Boulet(carte, tireur.orientation, tireur);
            boulet2.Apparaitre(posX + 8, posY);
            boulets.Add(boulet2);
        }
    }
}
