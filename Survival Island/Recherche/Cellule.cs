using Survival_Island.Entites.Base;

namespace Survival_Island.Recherche
{
    public class Cellule
    {
        public int GrillePosX { get; }
        public int GrillePosY { get; }

        public Cellule Parent { get; set; }
        public double CoutG { get; set; }
        public double CoutH { get; set; }
        public double CoutF { get { return CoutG + CoutH; } }

        public List<EntiteBase> Entites { get; }

        public Cellule(int grillePosX, int grillePosY)
        {
            GrillePosX = grillePosX;
            GrillePosY = grillePosY;

            CoutG = double.PositiveInfinity;
            CoutH = 0;

            Entites = new List<EntiteBase>();
        }

        public bool EstOccupee()
        {
            return Entites.Count > 0;
        }
    }
}
