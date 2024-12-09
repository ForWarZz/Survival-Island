using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Survival_Island
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public int NOMBRE_IMAGE_MER, IM_MER_LARG, IM_MER_HAUT;
        public Image[] laMer;
        public BitmapImage mer = new BitmapImage(new Uri("pack://application:,,,./tile_73.png"));

        public MainWindow()
        {
            InitializeComponent();
            IM_MER_LARG = (int)can.Width / (int)mer.Width;
            IM_MER_HAUT = (int)can.Height / (int)mer.Height;
            NOMBRE_IMAGE_MER = IM_MER_HAUT * IM_MER_LARG;
            laMer = new Image[NOMBRE_IMAGE_MER];
            InitCarte();
        }

        private void InitCarte()
        {
            for (int i = 0; i < IM_MER_HAUT; i++)
            {
                for (int j = 0; j < IM_MER_LARG; j++)
                {
                    laMer[i] = new Image();
                    laMer[i].Source = mer;
                    laMer[i].Width = mer.Width;
                    laMer[i].Height = mer.Height;
                    Canvas.SetLeft(laMer[i], j * mer.Width);
                    Canvas.SetTop(laMer[i], i * mer.Height);
                    can.Children.Add(laMer[i]);
                }
            }
        }
    }
}