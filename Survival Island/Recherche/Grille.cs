using Survival_Island.Entites.Base;

namespace Survival_Island.Recherche
{
    public class Grille
    {
        public Cellule[,] AStarGrille { get; }
        private int tailleCellule;

        private int largeur, hauteur;

        public Grille(int largeurMonde, int hauteurMonde, int tailleCellule)
        {

            largeur = largeurMonde / tailleCellule + 1;
            hauteur = hauteurMonde / tailleCellule + 1;

            AStarGrille = new Cellule[largeur, hauteur];

            this.tailleCellule = tailleCellule;

            for (int i = 0; i < largeur; i++)
            {
                for (int j = 0; j < hauteur; j++)
                {
                    AStarGrille[i, j] = new Cellule(i, j, i * tailleCellule, j * tailleCellule);
                }
            }
        }

        public bool EstOccupee(double posX, double posY)
        {
            return ObtenirCellule(posX, posY).EstOccupee();
        }

        public void AjouterEntiteDansGrille(EntiteBase entite)
        {
            double posX = entite.PositionX;
            double posY = entite.PositionY;
            double largeurEntite = entite.CanvaElement.Width;
            double hauteurEntite = entite.CanvaElement.Height;

            int debutX = (int)(posX / tailleCellule);
            int debutY = (int)(posY / tailleCellule);
            int finX = (int)((posX + largeurEntite) / tailleCellule);
            int finY = (int)((posY + hauteurEntite) / tailleCellule);

            for (int x = debutX; x <= finX; x++)
            {
                for (int y = debutY; y <= finY; y++)
                {
                    if (EstDansGrille(x, y))
                    {
                        AStarGrille[x, y].Entites.Add(entite);
                    }
                }
            }
        }

        public void SupprimerEntiteDansGrille(EntiteBase entite)
        {
            double posX = entite.PositionX;
            double posY = entite.PositionY;
            double largeurEntite = entite.CanvaElement.Width;
            double hauteurEntite = entite.CanvaElement.Height;

            int debutX = (int)(posX / tailleCellule);
            int debutY = (int)(posY / tailleCellule);
            int finX = (int)((posX + largeurEntite) / tailleCellule);
            int finY = (int)((posY + hauteurEntite) / tailleCellule);

            for (int x = debutX; x <= finX; x++)
            {
                for (int y = debutY; y <= finY; y++)
                {
                    if (EstDansGrille(x, y))
                    {
                        AStarGrille[x, y].Entites.Remove(entite);
                    }
                }
            }
        }

        public Cellule ObtenirCellule(double posX, double posY)
        {
            int grilleX = (int)(posX / tailleCellule);
            int grilleY = (int)(posY / tailleCellule);

            if (!EstDansGrille(grilleX, grilleY))
                throw new IndexOutOfRangeException("La position est hors de la grille: X=" + grilleX + " Y=" + grilleY + " TAILLE GRILLE=" + largeur);

            return AStarGrille[grilleX, grilleY];
        }

        public List<Cellule> ObtenirVoisins(Cellule cellule)
        {
            List<Cellule> voisins = new List<Cellule>();

            // Directions possibles (haut, bas, gauche, droite, et les diagonales)
            int[,] directions = new int[,]
            {
                { -1,  0 }, // Haut
                {  1,  0 }, // Bas
                {  0, -1 }, // Gauche
                {  0,  1 }, // Droite
                { -1, -1 }, // Diagonale haut-gauche
                { -1,  1 }, // Diagonale haut-droite
                {  1, -1 }, // Diagonale bas-gauche
                {  1,  1 }  // Diagonale bas-droite
            };

            int x = cellule.GrillePosX;
            int y = cellule.GrillePosY;

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int voisinX = x + directions[i, 0];
                int voisinY = y + directions[i, 1];

                if (EstDansGrille(voisinX, voisinY))
                {
                    voisins.Add(AStarGrille[voisinX, voisinY]);
                }
            }

            return voisins;
        }

        public bool EstDansGrille(int grilleX, int grilleY)
        {
            return grilleX >= 0 && grilleX < largeur && grilleY >= 0 && grilleY < hauteur;
        }

        public void AfficherGrille()
        {
            for (int i = 0; i < largeur; i++)
            {
                for (int j = 0; j < hauteur; j++)
                {
                    bool valeur = AStarGrille[i, j].EstOccupee();

                    if (valeur)
                        Console.Write("X ");
                    else
                        Console.Write("- ");
                }

                Console.WriteLine();
            }

            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        }
    }
}
