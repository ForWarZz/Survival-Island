using Survival_Island.Outils;
using System.Windows;
using System.Windows.Controls;

namespace Survival_Island.Entites.Base
{
    public class EntiteAvecVie : EntiteBase
    {
        public int Vie { get; set; }
        public int VieMax { get; set; }

        public ProgressBar BarreDeVie { get; private set; }

        private bool barreVieVisible;

        public EntiteAvecVie(Canvas carte, bool estStatique, bool barreVieVisible, int vieMax) : base(carte, estStatique)
        {
            VieMax = vieMax;
            Vie = vieMax;

            this.barreVieVisible = barreVieVisible;
        }

        public void AfficherBarreDeVie(bool afficher)
        {
            if (barreVieVisible)
                BarreDeVie.Visibility = afficher ? Visibility.Visible : Visibility.Hidden;
        }

        private void MettreAJourBarreDeVie()
        {
            if (barreVieVisible)
            {
                BarreDeVie.Visibility = Visibility.Visible;
                BarreDeVie.Maximum = VieMax;
                BarreDeVie.Value = Vie;
            }
        }

        public void MettreAJourPositionVie()
        {
            Canvas.SetLeft(BarreDeVie, PositionX + (CanvaElement.Width - BarreDeVie.Width) / 2);
            Canvas.SetTop(BarreDeVie, PositionY + CanvaElement.Height);
        }

        public override void Apparaitre(double x, double y)
        {
            base.Apparaitre(x, y);

            if (barreVieVisible)
            {
                BarreDeVie = new ProgressBar();

                BarreDeVie.Visibility = Visibility.Hidden;
                BarreDeVie.Width = CanvaElement.Width / 2;
                BarreDeVie.Height = Constante.HAUTEUR_BARRE_VIE;
                BarreDeVie.Minimum = 0;

                Carte.Children.Add(BarreDeVie);
                
                MettreAJourPositionVie();
            }

            MettreAJour();
        }

        public override void Disparaitre()
        {
            base.Disparaitre();

            if (barreVieVisible)
                Carte.Children.Remove(BarreDeVie);
        }

        public virtual bool InfligerDegats(int degats)
        {
            Vie -= degats;
            if (Vie < 0) Vie = 0;

            MettreAJour();

            if (Vie <= 0)
            {
                Disparaitre();
                return true;
            }

            return false;
        }

        public virtual void AjouterVie(int vie)
        {
            Vie = Math.Min(VieMax, Vie + vie);
            MettreAJour();
        }

        public virtual void MettreAJour()
        {
            MettreAJourBarreDeVie();
        }
    }
}
