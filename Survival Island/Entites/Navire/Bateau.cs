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
        protected MoteurJeu MoteurJeu { get; }

        public double VitesseMax { get; set; }
        public double VitesseActuelle { get; set; }

        public int Degats { get; set; }

        public double TempsRechargementCanon { get; set; }
        public double TempsDernierTir { get; set; }

        public bool Deplacement { get; set; }
        public bool CanonActif { get; set; }

        public Vector Orientation { get; set; }

        protected double AngleCible { get; set; }
        protected double AngleActuel { get; set; }

        protected DispatcherTimer rotationTemps;

        protected BitmapImage bateauImage;

        protected ModeBateau ModeBateau { get; set; }

        protected Bateau(Canvas carte, MoteurJeu moteurJeu, BitmapImage bitmapBateau, bool barreVie, int vieMax, int degats, double vitesse, double tempsRechargementCanon) : base(carte, false, barreVie, vieMax)
        {
            MoteurJeu = moteurJeu;

            Degats = degats;
            VitesseMax = vitesse;
            TempsRechargementCanon = tempsRechargementCanon;

            TempsDernierTir = 0;

            CanvaElement = new Image();
            ((Image)CanvaElement).Source = bitmapBateau;
            CanvaElement.Width = Constante.LARGEUR_NAVIRE;
            CanvaElement.Height = Constante.HAUTEUR_NAVIRE;

            InitRotationTemps();
        }

        public virtual void Deplacer()
        {
            if (Deplacement)
                VitesseActuelle = Math.Min(VitesseActuelle + Constante.BATEAU_ACCELERATION, VitesseMax); // Accélération progressive jusqu'à la vitesse maximale
            else
                VitesseActuelle = Math.Max(VitesseActuelle - Constante.BATEAU_ACCELERATION, 0); // Décélération progressive jusqu'à l'arrêt

            double nouvellePosX = PositionX + VitesseActuelle * Orientation.X;
            double nouvellePosY = PositionY + VitesseActuelle * Orientation.Y;

            double maxX = Carte.Width - CanvaElement.Width;
            double maxY = Carte.Height - CanvaElement.Height;

            if (PeutAllerVers(nouvellePosX, nouvellePosY))
            {
                PositionX = Math.Max(0, Math.Min(nouvellePosX, maxX));
                PositionY = Math.Max(0, Math.Min(nouvellePosY, maxY));
            } else
                VitesseActuelle = 0;
            
  
        }

        public virtual void ChangerOrientation(Point nouvellePosition)
        {
            double centreBateauX = PositionX + CanvaElement.Width / 2;
            double centreBateauY = PositionY + CanvaElement.Height / 2;

            double deltaX = nouvellePosition.X - centreBateauX;
            double deltaY = nouvellePosition.Y - centreBateauY;

            AngleCible = Math.Atan2(deltaY, deltaX) * 180 / Math.PI - 90;

            if (!rotationTemps.IsEnabled)
            {
                rotationTemps.Start();
            }
        }

        public virtual void TirerBoulet()
        {
            double centreBateauX = PositionX + CanvaElement.Width / 2;
            double centreBateauY = PositionY + CanvaElement.Height / 2;

            if (TempsDernierTir > 0)
                return;
            TempsDernierTir = TempsRechargementCanon;

            ModeBateau.Tirer(centreBateauX, centreBateauY);
        }

        protected bool PeutAllerVers(double nouvellePosX, double nouvellePosY)
        {
            Collision nouvelleCollision = new Collision(new Rect(nouvellePosX, nouvellePosY, CanvaElement.Width, CanvaElement.Height), AngleRotation());
            // nouvelleCollision.AfficherCollision(carte);

            if (MoteurJeu.Ile.EnCollisionAvec(nouvelleCollision))
                return false;

            foreach (Obstacle obstacle in MoteurJeu.Obstacles)
            {
                if (obstacle.EnCollisionAvec(nouvelleCollision))
                    return false;
            }

            foreach (ObjetRecompense objetBonus in MoteurJeu.ObjetsBonus)
            {
                if (objetBonus.EnCollisionAvec(nouvelleCollision))
                    return false;
            }

            return true;
        }

        private void AnimationRotation(object? sender, EventArgs e)
        {
            double diffAngle = AngleCible - AngleActuel;

            if (diffAngle > 180)
                diffAngle -= 360;

            if (diffAngle < -180)
                diffAngle += 360;

            if (Math.Abs(diffAngle) <= Constante.TOLERANCE_ANGLE_ROTATION)   // On ajoute une tolérance pour éviter les oscillations infinies
            {
                AngleActuel = AngleCible;
                rotationTemps.Stop();
            }
            else
            {
                AngleActuel += Math.Sign(diffAngle) * 5;
            }

            // On calcule le vetceur directeur (utilisé pour le tir des boulets ou encore le mouvement)
            double angleRad = (AngleActuel + 90) * Math.PI / 180;
            Orientation = new Vector(Math.Cos(angleRad), Math.Sin(angleRad));
            Orientation.Normalize();

            CanvaElement.RenderTransform = new RotateTransform(AngleActuel, CanvaElement.Width / 2, CanvaElement.Height / 2);
        }

        protected void InitRotationTemps()
        {
            rotationTemps = new DispatcherTimer();
            rotationTemps.Interval = TimeSpan.FromMilliseconds(Constante.TEMPS_ROTATION_NAVIRE);
            rotationTemps.Tick += AnimationRotation;
        }
    }

}
