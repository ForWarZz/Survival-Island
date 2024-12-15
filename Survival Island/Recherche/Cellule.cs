using Survival_Island.Entites.Base;

namespace Survival_Island.Recherche
{
    public class Cellule
    {
        public int GrillePosX { get; }
        public int GrillePosY { get; }

        public int MondePosX { get; }
        public int MondePosY { get; }

        public Cellule Parent { get; set; }
        public List<Cellule> Voisins { get; set; }

        public double CoutG { get; set; }
        public double CoutH { get; set; }
        public double CoutF { get { return CoutG + CoutH; } }

        public List<EntiteBase> Entites { get; }

        public Cellule(int grillePosX, int grillePosY, int mondePosX, int mondePosY)
        {
            GrillePosX = grillePosX;
            GrillePosY = grillePosY;

            CoutG = double.PositiveInfinity;
            CoutH = 0;

            Entites = new List<EntiteBase>();
            MondePosX = mondePosX;
            MondePosY = mondePosY;
        }

        public bool EstOccupee()
        {
            return Entites.Count > 0;
        }
    }
}
