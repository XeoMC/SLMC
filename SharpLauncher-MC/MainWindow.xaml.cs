using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using MojangAPI.Model;
using Newtonsoft.Json;
using xmc;
using xmc.uc;

namespace SharpLauncher_MC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public static HttpClient httpClient = new HttpClient();
        public static WebClient webClient = new WebClient();
        public static Mojang mojang = new Mojang(httpClient);
        public MainWindow()
        {
            InitializeComponent();
            this.Title = $"SLMC, build {Assembly.GetExecutingAssembly().GetName().Version.Revision}";
            Config.i = new Config();
            
            usernameText.Text = "__tacoguy";
            /*
            Console.WriteLine(usernameText.Text);
            PlayerUUID uid = mojang.GetUUID(usernameText.Text).GetAwaiter().GetResult();
            Console.WriteLine(uid.UUID);
            PlayerProfile p = mojang.GetProfileUsingUUID(uid.UUID).GetAwaiter().GetResult();

            MemoryStream skinData = new MemoryStream(webClient.DownloadData(new Uri(p.Skin.Url)));
            Bitmap skin = new Bitmap(skinData);
            skin = skin.Clone(new System.Drawing.Rectangle(8, 8, 8, 8), skin.PixelFormat);

            skinData = new MemoryStream();
            skin.Save(skinData, System.Drawing.Imaging.ImageFormat.Bmp);
            skinData.Position = 0;
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = skinData;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            Skin.Source = bitmapimage;
            */
            new Thread(() => {
                while(true)
                {
                    GC.Collect();
                    Thread.Sleep(TimeSpan.FromSeconds(60));
                }
            }).Start();
        }
        public bool settingsOpen = false;
        public void toggleSettings(bool? isOpen = null)
        {
            bool iO = true;
            if (isOpen == null) iO = !settingsOpen;
            else iO = (bool)isOpen;
            settingsOpen = iO;
            if (iO)
            {
                Settings.Visibility = Visibility.Visible;
                OverlaySettings.Visibility = Visibility.Visible;
                ThicknessAnimation a = new ThicknessAnimation { To = new Thickness(0), Duration = zGlobals.animationSpeed, DecelerationRatio = 1 };
                DoubleAnimation a1 = new DoubleAnimation { To = 0.25, Duration = zGlobals.animationSpeed };
                Settings.BeginAnimation(MarginProperty, a);
                OverlaySettings.BeginAnimation(OpacityProperty, a1);
            }
            else
            {
                ThicknessAnimation a = new ThicknessAnimation { To = new Thickness(-Settings.ActualWidth, 0, Settings.ActualWidth, 0), Duration = zGlobals.animationSpeed, AccelerationRatio = 1 };
                a.Completed += (s, e) =>
                {
                    Settings.Visibility = Visibility.Hidden;
                    OverlaySettings.Visibility = Visibility.Hidden;
                };
                DoubleAnimation a1 = new DoubleAnimation { To = 0, Duration = zGlobals.animationSpeed };
                Settings.BeginAnimation(MarginProperty, a);
                OverlaySettings.BeginAnimation(OpacityProperty, a1);
            }
        }
        public bool profileConfigOpen = false;
        public void toggleProfileConfig(bool? isOpen = null)
        {
            bool iO = true;
            if (isOpen == null) iO = !profileConfigOpen;
            else iO = (bool)isOpen;
            profileConfigOpen = iO;
            if (iO)
            {
                ProfileConfig.Visibility = Visibility.Visible;
                ThicknessAnimation a = new ThicknessAnimation { To = new Thickness(0), Duration = zGlobals.animationSpeed, DecelerationRatio = 1 };
                ProfileConfig.BeginAnimation(MarginProperty, a);
            }
            else
            {
                ThicknessAnimation a = new ThicknessAnimation { To = new Thickness(-ProfileConfig.ActualWidth, 0, ProfileConfig.ActualWidth, 0), Duration = zGlobals.animationSpeed, AccelerationRatio = 1 };
                a.Completed += (s, e) =>
                {
                    ProfileConfig.Visibility = Visibility.Hidden;
                };
                ProfileConfig.BeginAnimation(MarginProperty, a);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(true)
            {
                e.Cancel = true;
                toggleSettings();
//                toggleProfileConfig();
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

        private void Play_click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
