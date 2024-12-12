using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Survival_Island.Outils.Entites;

namespace Survival_Island.carte.objets
{
    public class Obstacle : EntiteBase
    {
        public Obstacle(Canvas carte, Image image) : base(carte, true)
        {
            canvaElement = image;
        }
    }
}
