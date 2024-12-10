using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Survival_Island
{
    internal class Boulet
    {
        public Image image { get; set; }
        public Vector direction { get; set; }

        public Boulet(Image image, Vector direction)
        {
            this.image = image;
            this.direction = direction;
        }
    }
}
