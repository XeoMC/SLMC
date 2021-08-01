using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MojangAPI;
using xmc;
using xmc.uc;

namespace SharpLauncher_MC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Title = $"SLMC, build {Assembly.GetExecutingAssembly().GetName().Version.Revision}";
        }
        public void toggleSettings(bool? isOpen = null)
        {
            bool iO = true;
            if (isOpen == null) iO = !Config.i.settingsOpen;
            else iO = (bool)isOpen;
            Config.i.settingsOpen = iO;
            if (iO)
            {
                Settings.Visibility = Visibility.Visible;
                Overlay.Visibility = Visibility.Visible;
                ThicknessAnimation a = new ThicknessAnimation { To = new Thickness(0), Duration = zGlobals.animationSpeed, DecelerationRatio = 1 };
                DoubleAnimation a1 = new DoubleAnimation { To = 0.25, Duration = zGlobals.animationSpeed };
                Settings.BeginAnimation(MarginProperty, a);
                Overlay.BeginAnimation(OpacityProperty, a1);
            } else
            {
                ThicknessAnimation a = new ThicknessAnimation { To = new Thickness(-Settings.ActualWidth, 0, Settings.ActualWidth, 0), Duration = zGlobals.animationSpeed, AccelerationRatio = 1 };
                a.Completed += (s, e) =>
                {
                    Settings.Visibility = Visibility.Hidden;
                    Overlay.Visibility = Visibility.Hidden;
                };
                DoubleAnimation a1 = new DoubleAnimation { To = 0, Duration = zGlobals.animationSpeed };
                Settings.BeginAnimation(MarginProperty, a);
                Overlay.BeginAnimation(OpacityProperty, a1);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(true)
            {
                e.Cancel = true;
                toggleSettings();
            }
        }

        protected bool winSizeFix = false;
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(!winSizeFix)
            {
                this.MinWidth += this.Width - Workspace.ActualWidth;
                this.MinHeight += this.Height - Workspace.ActualHeight;
                winSizeFix = true;
            }
//            this.Title = $"{Workspace.ActualWidth} < {this.MinWidth} = {(Workspace.ActualWidth < this.MinWidth).ToString().ToUpper()}";
            if ((Play.ActualWidth == Play.MaxWidth && !Config.i.LargeMode) || (e.NewSize.Width - this.Width + Workspace.ActualWidth < Play.MaxWidth * 0.6 && Config.i.LargeMode))
                toggleLargeMode(!Config.i.LargeMode);
        }

        private void toggleLargeMode(bool? isLarge = null)
        {
            bool iL = false;
            if (isLarge == null) iL = !Config.i.LargeMode;
            else iL = (bool)isLarge;
            if(iL)
            {
                Config.i.LargeMode = true;
                DoubleAnimation a = new DoubleAnimation { From = Play.ActualWidth, To = Play.MaxWidth * 0.6, Duration = zGlobals.animationSpeed.Multiply(3), DecelerationRatio = 1 };
                Play.BeginAnimation(WidthProperty, a);
                DoubleAnimation a1 = new DoubleAnimation
                {
                    To = Settings.MaxWidth / 1.5,
                    Duration = zGlobals.animationSpeed,
                    DecelerationRatio = 1
                };
                Settings.BeginAnimation(MaxWidthProperty, a1);
            } else
            {
                DoubleAnimation a1 = new DoubleAnimation
                {
                    To = Settings.MaxWidth * 1.5,
                    Duration = zGlobals.animationSpeed,
                    AccelerationRatio = 1
                };
                Settings.BeginAnimation(MaxWidthProperty, a1);
                Config.i.LargeMode = false;
                Play.BeginAnimation(WidthProperty, null);
                Play.Width = double.NaN;
            }
        }

        private void Overlay_MouseUp(object sender, MouseButtonEventArgs e)
        {
            toggleSettings(false);
        }
    }
}
