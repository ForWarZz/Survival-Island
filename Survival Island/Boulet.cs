using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Survival_Island
{
    internal class Boulet
    {
        public Ellipse boulet { get; set; }
        public Vector direction { get; set; }

        public Boulet(Ellipse boulet, Vector direction)
        {
            this.boulet = boulet;
            this.direction = direction;
        }
    }
}
