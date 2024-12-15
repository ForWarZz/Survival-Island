using Survival_Island.Outils;
using System.Windows;
using System.Windows.Controls;

namespace Survival_Island.Entites.Base
{
    public abstract class EntiteAvecVie : EntiteBase
    {
        public int vie { get; set; }
        public int vieMax { get; set; }

        public ProgressBar barreDeVie { get; set; }

        private bool barreVieVisible;

        public EntiteAvecVie(Canvas carte, bool estStatique, bool barreVieVisible, int vieMax) : base(carte, estStatique)
        {
            this.vieMax = vieMax;
            vie = vieMax;

            this.barreVieVisible = barreVieVisible;
        }

        public void AfficherBarreDeVie(bool afficher)
        {
            if (barreVieVisible)
                barreDeVie.Visibility = afficher ? Visibility.Visible : Visibility.Hidden;
        }

        public void MettreAJourBarreDeVie()
        {
            if (barreVieVisible)
            {
                barreDeVie.Visibility = Visibility.Visible;
                barreDeVie.Maximum = vieMax;
                barreDeVie.Value = vie;
            }
        }

        public override void Apparaitre(double x, double y)
        {
            base.Apparaitre(x, y);

            if (barreVieVisible)
            {
                barreDeVie = new ProgressBar();

                barreDeVie.Visibility = Visibility.Hidden;
                barreDeVie.Width = CanvaElement.Width / 2;
                barreDeVie.Height = Constante.HAUTEUR_BARRE_VIE;
                barreDeVie.Minimum = 0;

                carte.Children.Add(barreDeVie);

                Canvas.SetLeft(barreDeVie, x + (CanvaElement.Width - barreDeVie.Width) / 2);
                Canvas.SetTop(barreDeVie, y + CanvaElement.Height);
            }
        }

        public override void Disparaitre()
        {
            base.Disparaitre();

            if (barreVieVisible)
                carte.Children.Remove(barreDeVie);
        }

        public virtual bool InfligerDegats(int degats)
        {
            vie -= degats;
            if (vie < 0) vie = 0;

            MettreAJourBarreDeVie();

            if (vie <= 0)
            {
                Disparaitre();
                return true;
            }

            return false;
        }

        public virtual void AjouterVie(int vie)
        {
            this.vie = Math.Min(vieMax, this.vie + vie);
            MettreAJourBarreDeVie();
        }
    }
}
