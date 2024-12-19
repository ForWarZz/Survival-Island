using Survival_Island.Entites.Base;
using Survival_Island.Entites.Objets;
using Survival_Island.Outils;
using System.Media;
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

        public int TailleBoulets {  get; set; }

        public string ModeBateau { get; set; }

        protected int nombreBouletsParShoot;
        protected int angleBoulets;
        protected int espacementBoulets;

        private Brush couleurBoulet;

        protected Bateau(MoteurJeu moteurJeu, BitmapImage bitmapBateau, Brush couleurBoulet, bool barreVie, int vieMax, int degats, double vitesse, double tempsRechargementCanon) : base(moteurJeu.Carte, false, barreVie, vieMax)
        {
            MoteurJeu = moteurJeu;

            Degats = degats;
            VitesseMax = vitesse;
            TempsRechargementCanon = tempsRechargementCanon;

            TempsDernierTir = 0;

            this.couleurBoulet = couleurBoulet;

            CanvaElement = new Image();
            ((Image)CanvaElement).Source = bitmapBateau;
            CanvaElement.Width = Constante.LARGEUR_NAVIRE;
            CanvaElement.Height = Constante.HAUTEUR_NAVIRE;

            TailleBoulets = Constante.TAILLE_BOULET_INIT;

            nombreBouletsParShoot = 1;
            espacementBoulets = 0;
            angleBoulets = 0;

            ModeBateau = "classique";

            InitRotationTemps();
        }

        public virtual void Deplacer(double deltaTemps)
        {
            if (!Deplacement && VitesseActuelle == 0)
                return;

            if (Deplacement)
                VitesseActuelle = Math.Min(VitesseActuelle + Constante.BATEAU_ACCELERATION * deltaTemps, VitesseMax); // Accélération progressive jusqu'à la vitesse maximale
            else
                VitesseActuelle = Math.Max(VitesseActuelle - Constante.BATEAU_ACCELERATION * deltaTemps, 0); // Décélération progressive jusqu'à l'arrêt

            double nouvellePosX = PositionX + VitesseActuelle * Orientation.X * deltaTemps;
            double nouvellePosY = PositionY + VitesseActuelle * Orientation.Y * deltaTemps;

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
            double deltaX = nouvellePosition.X - Centre.X;
            double deltaY = nouvellePosition.Y - Centre.Y;

            AngleCible = Math.Atan2(deltaY, deltaX) * 180 / Math.PI - 90;

            if (!rotationTemps.IsEnabled)
            {
                rotationTemps.Start();
            }
        }

        protected abstract bool PeutAllerVers(double nouvellePosX, double nouvellePosY);

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

        public void TirerBoulet()
        {
            double centreBateauX = PositionX + CanvaElement.Width / 2;
            double centreBateauY = PositionY + CanvaElement.Height / 2;

            if (TempsDernierTir > 0)
                return;
            TempsDernierTir = TempsRechargementCanon;

            QuandBouletTire();

            double espaceBoulets = angleBoulets / (nombreBouletsParShoot);

            double angleOrientation = Math.Atan2(Orientation.Y, Orientation.X);

            double espacementBouletsX = 0;
            double espacementBouletsY = 0;

            if (espacementBoulets > 0)
            {
                // Calcul du décalage en X et Y
                espacementBouletsX = -Math.Sin(angleOrientation) * espacementBoulets;
                espacementBouletsY = Math.Cos(angleOrientation) * espacementBoulets;
            }

            for (int i = 0; i < nombreBouletsParShoot; i++)
            {
                double offsetAngle = (i - (nombreBouletsParShoot) / 2.0) * (espaceBoulets * Math.PI / 180.0);
                double angleBoulet = angleOrientation + offsetAngle;

                // Calcul de la direction du boulet
                Vector directionBoulet = new Vector(Math.Cos(angleBoulet), Math.Sin(angleBoulet));


                // Création et apparition du boulet
                Boulet boulet = new Boulet(Carte, directionBoulet, this, couleurBoulet);


                Point positionBoulet = new Point(
                    centreBateauX - boulet.CanvaElement.Width / 2 + i * espacementBouletsX,
                    centreBateauY - boulet.CanvaElement.Height / 2 + i * espacementBouletsY);

                boulet.Apparaitre(positionBoulet);
                MoteurJeu.Boulets.Add(boulet);
            }
        }

        protected virtual void QuandBouletTire()
        {

        }
    }
}
