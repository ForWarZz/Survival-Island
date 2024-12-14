using Survival_Island;
using Survival_Island.Entites.Base;
using Survival_Island.Entites.Objets;
using Survival_Island.Outils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Survival_Island.Entites.Navire
{
    public abstract class Bateau : EntiteAvecVie
    {
        protected MoteurJeu moteurJeu { get; set; }

        public double vitesse { get; set; }
        public int degats { get; set; }

        public double tempsRechargementCanon { get; set; }
        public double tempsDernierTir { get; set; }

        public bool deplacement { get; set; }
        public bool canonActif { get; set; }

        public Vector orientation { get; protected set; }

        protected double angleCible { get; set; }
        protected double angleActuel { get; set; }

        protected DispatcherTimer rotationTemps;

        protected BitmapImage bateauImage;

        protected Bateau(Canvas carte, MoteurJeu moteurJeu, BitmapImage bitmapBateau, bool barreVie, int vieMax, int degats, double vitesse, double tempsRechargementCanon) : base(carte, false, barreVie, vieMax)
        {
            this.moteurJeu = moteurJeu;

            this.degats = degats;
            this.vitesse = vitesse;
            this.tempsRechargementCanon = tempsRechargementCanon;

            tempsDernierTir = 0;

            canvaElement = new Image();
            ((Image)canvaElement).Source = bitmapBateau;
            canvaElement.Width = Constante.LARGEUR_NAVIRE;
            canvaElement.Height = Constante.HAUTEUR_NAVIRE;

            InitRotationTemps();
        }

        public abstract void Deplacer(double deltaX, double deltaY);

        public abstract void ChangerOrientation(Point nouvellePosition);

        public abstract void TirerBoulet();

        protected bool PeutAllerVers(double nouvellePosX, double nouvellePosY)
        {
            Collision nouvelleCollision = new Collision(new Rect(nouvellePosX, nouvellePosY, canvaElement.Width, canvaElement.Height), AngleRotation());

            if (moteurJeu.ile.EnCollisionAvec(nouvelleCollision))
                return false;

            foreach (Obstacle obstacle in moteurJeu.obstacles)
            {
                if (obstacle.EnCollisionAvec(nouvelleCollision))
                    return false;
            }

            foreach (ObjetRecompense objetBonus in moteurJeu.objetsBonus)
            {
                if (objetBonus.EnCollisionAvec(nouvelleCollision))
                    return false;
            }

            return true;
        }

        private void AnimationRotation(object? sender, EventArgs e)
        {
            double diffAngle = angleCible - angleActuel;

            if (diffAngle > 180)
                diffAngle -= 360;

            if (diffAngle < -180)
                diffAngle += 360;

            if (Math.Abs(diffAngle) <= Constante.TOLERANCE_ANGLE_ROTATION)   // On ajoute une tolérance pour éviter les oscillations infinies
            {
                angleActuel = angleCible;
                rotationTemps.Stop();
            }
            else
            {
                angleActuel += Math.Sign(diffAngle) * 5;
            }

            // On calcule le vetceur directeur (utilisé pour le tir des boulets ou encore le mouvement)
            double angleRad = (angleActuel + 90) * Math.PI / 180;
            orientation = new Vector(Math.Cos(angleRad), Math.Sin(angleRad));
            orientation.Normalize();

            canvaElement.RenderTransform = new RotateTransform(angleActuel, canvaElement.Width / 2, canvaElement.Height / 2);
        }

        protected void InitRotationTemps()
        {
            rotationTemps = new DispatcherTimer();
            rotationTemps.Interval = TimeSpan.FromMilliseconds(Constante.TEMPS_ROTATION_NAVIRE);
            rotationTemps.Tick += AnimationRotation;
        }
    }

}
