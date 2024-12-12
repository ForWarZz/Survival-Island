using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Survival_Island.Outils
{
    public interface IDetruisable
    {
        public int vieMax { get; set; }
        public int vie { get; set; }

        abstract bool InfligerDegats(int degats);
    }
}
