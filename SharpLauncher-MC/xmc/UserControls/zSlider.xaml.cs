using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpLauncher_MC.xmc.UserControls
{
    /// <summary>
    /// Interaction logic for zSlider.xaml
    /// </summary>
    public partial class zSlider : UserControl
    {
        public zSlider()
        {
            InitializeComponent();
            SizeChanged += ZSlider_SizeChanged;
        }

        public event 

        private void ZSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        public static readonly DependencyProperty
            MinimumValueProperty = DependencyProperty.Register("MinimumValue", typeof(double), typeof(zSlider), new PropertyMetadata(0d, new PropertyChangedCallback());

        private static void MinimumValueChangedCallback()
        {

        }

        private void zButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
        private void zButton_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
