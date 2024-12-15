using Survival_Island.Entites.Base;

namespace Survival_Island.Recherche
{
    public class RechercheChemin
    {
        public Grille Grille { get; }

        public RechercheChemin(int largeurMonde, int hauteurMonde, int tailleCellule)
        {
            Grille = new Grille(largeurMonde, hauteurMonde, tailleCellule);
        }

        public List<Cellule> TrouverChemin(double aPosX, double aPosY, double bPosX, double bPosY)
        {
            Cellule pointDepart = Grille.ObtenirCellule(aPosX, aPosY);
            Cellule pointArrivee = Grille.ObtenirCellule(bPosX, bPosY);

            List<Cellule> ouvertes = new List<Cellule>();
            List<Cellule> fermees = new List<Cellule>();
            ouvertes.Add(pointDepart);

            while (ouvertes.Count > 0)
            {
                Cellule meilleurCout = ouvertes.OrderBy(c => c.CoutF).First();

                if (meilleurCout == pointArrivee)
                {
                    Console.WriteLine("Chemin trouvé");
                    return ConstruireChemin(pointArrivee);
                }

                ouvertes.Remove(meilleurCout);
                fermees.Add(meilleurCout);

                List<Cellule> voisins = Grille.ObtenirVoisins(meilleurCout);

                foreach (Cellule voisin in voisins)
                {
                    if (fermees.Contains(voisin) || voisin.EstOccupee())
                        continue;

                    double nouveauCoutG = meilleurCout.CoutG + Distance(meilleurCout, voisin);

                    if (nouveauCoutG < voisin.CoutG || !ouvertes.Contains(voisin))
                    {
                        voisin.CoutG = nouveauCoutG;
                        voisin.CoutH = Distance(voisin, pointArrivee);
                        voisin.Parent = meilleurCout;

                        if (!ouvertes.Contains(voisin))
                            ouvertes.Add(voisin);
                    }
                }
            }

            return [];
        }

        private List<Cellule> ConstruireChemin(Cellule cellule)
        {
            List<Cellule> chemin = new List<Cellule>();
            Cellule current = cellule;
            while (current != null)
            {
                chemin.Add(current);
                current = current.Parent;
            }

            chemin.Reverse();
            return chemin;
        }

        private int Distance(Cellule a, Cellule b)
        {
            return Math.Abs(a.GrillePosX - b.GrillePosX) + Math.Abs(a.GrillePosY - b.GrillePosY);
        }
    }
}
