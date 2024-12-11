using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Survival_Island.carte
{
    internal class Obstacle : Collision
    {
        public Obstacle(Canvas carte, Image image) : base(carte)
        {
            element = image;
        }
    }
}
