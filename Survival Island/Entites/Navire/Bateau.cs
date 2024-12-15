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

        public double vitesseMax { get; set; }
        public double vitesseActuelle { get; set; }

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
            this.vitesseMax = vitesse;
            this.tempsRechargementCanon = tempsRechargementCanon;

            tempsDernierTir = 0;

            CanvaElement = new Image();
            ((Image)CanvaElement).Source = bitmapBateau;
            CanvaElement.Width = Constante.LARGEUR_NAVIRE;
            CanvaElement.Height = Constante.HAUTEUR_NAVIRE;

            InitRotationTemps();
        }

        public virtual void Deplacer()
        {
            // Calcul de la vitesse en fonction des entrées utilisateur ou du comportement attendu
            if (deplacement)
            {
                vitesseActuelle = Math.Min(vitesseActuelle + Constante.BATEAU_ACCELERATION, vitesseMax); // Accélération progressive jusqu'à la vitesse maximale
            }
            else
            {
                vitesseActuelle = Math.Max(vitesseActuelle - Constante.BATEAU_ACCELERATION, 0); // Décélération progressive jusqu'à l'arrêt
            }

            double nouvellePosX = PositionX + vitesseActuelle * orientation.X;
            double nouvellePosY = PositionY + vitesseActuelle * orientation.Y;

            double maxX = carte.Width - CanvaElement.Width;
            double maxY = carte.Height - CanvaElement.Height;

            if (PeutAllerVers(nouvellePosX, nouvellePosY))
            {
                PositionX = Math.Max(0, Math.Min(nouvellePosX, maxX));
                PositionY = Math.Max(0, Math.Min(nouvellePosY, maxY));
            } else
            {
                vitesseActuelle = 0;
            }
  
        }

        public virtual void ChangerOrientation(Point nouvellePosition)
        {
            double centreBateauX = PositionX + CanvaElement.Width / 2;
            double centreBateauY = PositionY + CanvaElement.Height / 2;

            double deltaX = nouvellePosition.X - centreBateauX;
            double deltaY = nouvellePosition.Y - centreBateauY;

            angleCible = Math.Atan2(deltaY, deltaX) * 180 / Math.PI - 90;

            if (!rotationTemps.IsEnabled)
            {
                rotationTemps.Start();
            }
        }

        public abstract void TirerBoulet();

        protected bool PeutAllerVers(double nouvellePosX, double nouvellePosY)
        {
            Collision nouvelleCollision = new Collision(new Rect(nouvellePosX, nouvellePosY, CanvaElement.Width, CanvaElement.Height), AngleRotation());
            // nouvelleCollision.AfficherCollision(carte);

            if (moteurJeu.Ile.EnCollisionAvec(nouvelleCollision))
                return false;

            foreach (Obstacle obstacle in moteurJeu.Obstacles)
            {
                if (obstacle.EnCollisionAvec(nouvelleCollision))
                    return false;
            }

            foreach (ObjetRecompense objetBonus in moteurJeu.ObjetsBonus)
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

            CanvaElement.RenderTransform = new RotateTransform(angleActuel, CanvaElement.Width / 2, CanvaElement.Height / 2);
        }

        protected void InitRotationTemps()
        {
            rotationTemps = new DispatcherTimer();
            rotationTemps.Interval = TimeSpan.FromMilliseconds(Constante.TEMPS_ROTATION_NAVIRE);
            rotationTemps.Tick += AnimationRotation;
        }
    }

}
