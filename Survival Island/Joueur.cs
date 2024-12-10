
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
public class Joueur
{
    // Propriétés du joueur
    public double CentreXDuJoueur { get; set; }
    public double CentreYDuJoueur { get; set; }
    public double AngleActuelDuBateau { get; set; } = 0; // Angle actuel du bateau
    public double RotationSpeed { get; set; } = 5; // Vitesse de rotation (en degrés par tick)
    public bool CannonActif { get; set; } = false; // Pour suivre si le canon est actif
    public int TempsDernierShoot { get; set; } = 0;
    public int PasDeplacement { get; set; } = 3;
    public int VitesseRechargement { get; set; } = 10;

    public DispatcherTimer MinuterieMouvement;
    public DispatcherTimer MinuterieGlobal;

    // État de mouvement
    public bool MouvementHautBolleen { get; set; } = false;
    public bool MouvementBasBolleen { get; set; } = false;
    public bool MouvementGaucheBolleen { get; set; } = false;
    public bool MouvementDroitBolleen { get; set; } = false;

    // Références à l'image et au canvas
    public Image RedShip { get; set; }
    public Canvas Can1 { get; set; }
    public Point PositionSouris { get; set; }

    public Joueur(Image redShip, Canvas can1)
    {
        RedShip = redShip;
        Can1 = can1;

        // Calculer le centre de l'image (RedShip)
        CentreXDuJoueur = RedShip.Width / 2;
        CentreYDuJoueur = RedShip.Height / 2;

        InitMinuterieMouvement();
    }


    public void InitMinuterieMouvement()
    {
        MinuterieMouvement = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16) // Environ 60 FPS
        };
        MinuterieMouvement.Tick += MoveShip; // Attacher la logique de mouvement
        MinuterieMouvement.Start(); // Démarrer le minuteur de mouvement

        // Mettre à jour les coordonnées du centre du bateau après chaque mouvement
        CompositionTarget.Rendering += (s, e) =>
        {
            CentreXDuJoueur = RedShip.Width / 2;
            CentreYDuJoueur = RedShip.Height / 2;
        };
    }

    public void MoveShip(object? sender, EventArgs e)
    {
        if (MouvementHautBolleen)
        {
            Canvas.SetTop(RedShip, Canvas.GetTop(RedShip) - PasDeplacement);
        }
        if (MouvementBasBolleen)
        {
            Canvas.SetTop(RedShip, Canvas.GetTop(RedShip) + PasDeplacement);
        }
        if (MouvementGaucheBolleen)
        {
            Canvas.SetLeft(RedShip, Canvas.GetLeft(RedShip) - PasDeplacement);

        }
        if (MouvementDroitBolleen)
        {
            Canvas.SetLeft(RedShip, Canvas.GetLeft(RedShip) + PasDeplacement);
        }
    }

    public double CalculateAngle(Point mousePosition)
    {
        // Récupérer la position actuelle de l'image
        double imageLeft = Canvas.GetLeft(RedShip);
        double imageTop = Canvas.GetTop(RedShip);

        // Calculer les décalages entre la souris et le centre de l'image
        double deltaX = mousePosition.X - (imageLeft + CentreXDuJoueur);
        double deltaY = mousePosition.Y - (imageTop + CentreYDuJoueur);

        // Calculer l'angle en degrés (converti de radians) et ajuster pour orienter correctement
        return Math.Atan2(deltaY, deltaX) * (180 / Math.PI) - 90;
    }

    public double InterpolateAngle(double currentAngle, double targetAngle, double speed)
    {
        // Calculer la différence d'angle (delta) tout en gérant les cas de dépassement d'angle (> 180 ou < -180)
        double deltaAngle = targetAngle - currentAngle;

        if (deltaAngle > 180) deltaAngle -= 360;
        if (deltaAngle < -180) deltaAngle += 360;

        // Si la différence est inférieure à la vitesse, atteindre directement l'angle cible
        if (Math.Abs(deltaAngle) < speed)
            return targetAngle;

        // Ajuster l'angle actuel en fonction de la direction du delta et de la vitesse
        return currentAngle + Math.Sign(deltaAngle) * speed;
    }

    public void RotateImage(double angle)
    {
        // Appliquer la rotation à l'image avec un pivot au centre de l'image
        RedShip.RenderTransform = new RotateTransform(angle, CentreXDuJoueur, CentreYDuJoueur);
    }

    public void Shoot()
    {
        // Créer une nouvelle image pour la balle
        Ellipse bullet = new Ellipse
        {
            Width = 10,
            Height = 10,
            Fill = Brushes.Black
        };

        // Positionner la balle au centre du bateau
        Canvas.SetLeft(bullet, Canvas.GetLeft(RedShip) + CentreXDuJoueur - bullet.Width / 2);
        Canvas.SetTop(bullet, Canvas.GetTop(RedShip) + CentreYDuJoueur - bullet.Height / 2);

        // Ajouter la balle au canvas principal
        Can1.Children.Add(bullet);

        // Définir la trajectoire de la balle
        MoveBullet(bullet, AngleActuelDuBateau); // Passer l'angle actuel du bateau
    }

    public void MoveBullet(Ellipse bullet, double initialAngle)
    {
        DispatcherTimer bulletTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };

        bulletTimer.Tick += (s, e) =>
        {
            double speed = 10;
            double angleInRadians = (initialAngle + 90) * (Math.PI / 180); // Utiliser l'angle initial

            double deltaX = Math.Cos(angleInRadians) * speed;
            double deltaY = Math.Sin(angleInRadians) * speed;

            double currentLeft = Canvas.GetLeft(bullet);
            double currentTop = Canvas.GetTop(bullet);

            Canvas.SetLeft(bullet, currentLeft + deltaX);
            Canvas.SetTop(bullet, currentTop + deltaY);

            // Supprimer la balle si elle sort des limites
            if (currentLeft < 0 || currentLeft > Can1.ActualWidth ||
                currentTop < 0 || currentTop > Can1.ActualHeight)
            {
                Can1.Children.Remove(bullet);
                bulletTimer.Stop();
            }
        };

        bulletTimer.Start();
    }
}