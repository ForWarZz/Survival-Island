using Survival_Island.Entites.Base;
using System.Windows.Controls;

namespace Survival_Island.Entites.Objets
{
    public class Obstacle : EntiteBase
    {
        public Obstacle(Canvas carte, Image image) : base(carte, true)
        {
            CanvaElement = image;
        }
    }
}
