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
            Cellule? pointArrivee = Grille.ObtenirCellule(bPosX, bPosY);

            if (pointArrivee.EstOccupee())
            {
                pointArrivee = TrouverCelluleAccessibleLaPlusProche(pointArrivee);
                if (pointArrivee == null)
                    return new List<Cellule>();
            }

            PriorityQueue<Cellule, double> ouvertes = new PriorityQueue<Cellule, double>();
            HashSet<Cellule> fermees = new HashSet<Cellule>();

            ouvertes.Enqueue(pointDepart, 0);
            pointDepart.CoutG = 0;
            pointDepart.CoutH = Distance(pointDepart, pointArrivee);

            while (ouvertes.Count > 0)
            {
                Cellule meilleurCout = ouvertes.Dequeue();
                if (meilleurCout == pointArrivee)
                    return ConstruireChemin(pointArrivee);

                fermees.Add(meilleurCout);

                foreach (Cellule voisin in Grille.ObtenirVoisins(meilleurCout))
                {
                    if (fermees.Contains(voisin) || voisin.EstOccupee())
                        continue;

                    double tentativeG = meilleurCout.CoutG + Distance(meilleurCout, voisin);
                    if (tentativeG < voisin.CoutG || !ouvertes.UnorderedItems.Any(x => x.Element == voisin))
                    {
                        voisin.CoutG = tentativeG;
                        voisin.CoutH = Distance(voisin, pointArrivee);
                        voisin.Parent = meilleurCout;

                        ouvertes.Enqueue(voisin, voisin.CoutF);
                    }
                }
            }

            return [];
        }

        private Cellule? TrouverCelluleAccessibleLaPlusProche(Cellule celluleCible)
        {
            // Recherche en largeur pour trouver la cellule accessible la plus proche
            Queue<Cellule> aExplorer = new Queue<Cellule>();
            HashSet<Cellule> visitees = new HashSet<Cellule>();

            aExplorer.Enqueue(celluleCible);

            while (aExplorer.Count > 0)
            {
                Cellule actuelle = aExplorer.Dequeue();

                if (!visitees.Contains(actuelle))
                {
                    visitees.Add(actuelle);

                    if (!actuelle.EstOccupee())
                        return actuelle;

                    foreach (Cellule voisin in Grille.ObtenirVoisins(actuelle))
                    {
                        if (!visitees.Contains(voisin))
                        {
                            aExplorer.Enqueue(voisin);
                        }
                    }
                }
            }

            return null;
        }

        public void AfficherChemin(List<Cellule> cellulesChemin)
        {
            int lignes = Grille.AStarGrille.GetLength(0);
            int colonnes = Grille.AStarGrille.GetLength(1);

            char[,] grilleVisuelle = new char[lignes, colonnes];

            for (int i = 0; i < lignes; i++)
            {
                for (int j = 0; j < colonnes; j++)
                {
                    grilleVisuelle[i, j] = Grille.AStarGrille[i, j].EstOccupee() ? '.' : '#';
                }
            }

            foreach (Cellule cellule in cellulesChemin)
            {
                grilleVisuelle[cellule.GrillePosX, cellule.GrillePosY] = '*';
            }

            // Afficher la grille dans la console
            for (int i = 0; i < lignes; i++)
            {
                for (int j = 0; j < colonnes; j++)
                {
                    Console.Write(grilleVisuelle[i, j] + " ");
                }

                Console.WriteLine();
            }
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
            return (int)(Math.Pow(a.GrillePosX - b.GrillePosX, 2) + Math.Pow(a.GrillePosY - b.GrillePosY, 2));
        }
    }
}
