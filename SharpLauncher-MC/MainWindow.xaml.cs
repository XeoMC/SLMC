using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
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
        public static HttpClient HttpClient = new HttpClient();
        public static WebClient WebClient = new WebClient();
        public static Mojang Mojang = new Mojang(HttpClient);
        public static MojangAuth MojangAuth = new MojangAuth(HttpClient);
        public static Session CurrentSession;
        public static string GetLauncherOSName()
        {
            return "windows"; // because of wpf and dotnet framework
        }
        [DllImport("Libraries\\STDOuter.dll")]
        public static extern void StartWithLogger(string prc, string arg, string workDir);
        public MainWindow()
        {
            InitializeComponent();
            VersionText.Text = $"{Assembly.GetExecutingAssembly().GetName().Name} v{Assembly.GetExecutingAssembly().GetName().Version}";
            this.Title = $"SLMC, build {Assembly.GetExecutingAssembly().GetName().Version.Revision}";

            Config.i = new Config();
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.SLMC/SLMC.json"))
                Config.Load();
            else
            {
                Config.i = new Config();
                Config.Save();
            }
#if DEBUG
            new Thread(() => {
                while(true)
                {
                    GC.Collect();
                    Thread.Sleep(TimeSpan.FromSeconds(60));
                }
            }).Start();
            string text = File.ReadAllText($"{Config.i.minecraftPath}/versions/1.17.1/1.17.1.json");
            p = JsonConvert.DeserializeObject<SharpLauncher_MC.JSON.ClassicLauncher.ClassicVersion>(text, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }).ToProfile();
#endif
        }
        public async Task updateSkin()
        {
            if (CurrentSession == null) return;
            Dispatcher.Invoke(() => usernameText.Text = CurrentSession.Username);
            PlayerProfile p = await Mojang.GetProfileUsingUUID(CurrentSession.UUID);

            MemoryStream skinData = new MemoryStream(await WebClient.DownloadDataTaskAsync(new Uri(p.Skin.Url)));
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
            Dispatcher.Invoke(() => Skin.Source = bitmapimage);
            return;
        }
        public bool settingsOpen = false;
        public bool loginScreenOpen = true;
        public bool profileConfigOpen = false;
        public void toggleLogin(bool? isOpen = null) => toggleForm(Login, ref loginScreenOpen, isOpen);
        public void toggleSettings(bool? isOpen = null) => toggleForm(Settings, ref settingsOpen, isOpen);
        public void toggleForm(Grid form, ref bool stateProperty, bool? isOpen = null)
        {
            bool iO = !stateProperty;
            if (isOpen != null) iO = (bool)isOpen;
            stateProperty = iO;
            if (iO)
            {
                form.Visibility = Visibility.Visible;
                Overlay.Visibility = Visibility.Visible;
                ThicknessAnimation a = new ThicknessAnimation { To = new Thickness(0), Duration = zGlobals.animationSpeed, DecelerationRatio = 1 };
                DoubleAnimation a1 = new DoubleAnimation { To = 0.25, Duration = zGlobals.animationSpeed };
                form.BeginAnimation(MarginProperty, a);
                Overlay.BeginAnimation(OpacityProperty, a1);
            }
            else
            {
                ThicknessAnimation a = new ThicknessAnimation { To = new Thickness(-Settings.ActualWidth, 0, Settings.ActualWidth, 0), Duration = zGlobals.animationSpeed, AccelerationRatio = 1 };
                a.Completed += (s, e) =>
                {
                    form.Visibility = Visibility.Hidden;
                    Overlay.Visibility = Visibility.Hidden;
                };
                DoubleAnimation a1 = new DoubleAnimation { To = 0, Duration = zGlobals.animationSpeed };
                form.BeginAnimation(MarginProperty, a);
                Overlay.BeginAnimation(OpacityProperty, a1);
            }
        }
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
#if DEBUG
            e.Cancel = true;
            toggleSettings();
//            toggleProfileConfig();
#endif
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
            if ((Play.ActualWidth == Play.MaxWidth && !Config.i.largeMode) || (e.NewSize.Width - this.Width + Workspace.ActualWidth < Play.MaxWidth * 0.6 && Config.i.largeMode))
                toggleLargeMode(!Config.i.largeMode);
        }

        private void toggleLargeMode(bool? isLarge = null)
        {
            bool iL = false;
            if (isLarge == null) iL = !Config.i.largeMode;
            else iL = (bool)isLarge;
            if(iL)
            {
                Config.i.largeMode = true;
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
                Config.i.largeMode = false;
                Play.BeginAnimation(WidthProperty, null);
                Play.Width = double.NaN;
            }
        }

        private void Overlay_MouseUp(object sender, MouseButtonEventArgs e)
        {
            toggleSettings(false);
        }
#if DEBUG
        public JSON.Profile p;
#endif
        private void Play_click(object sender, RoutedEventArgs e)
        {
            p.Start();
        }

        private void showPass(object sender, RoutedEventArgs e) => loginPassword.IsPass = !loginPassword.IsPass;

        private void Hyperlink_Click(object sender, RoutedEventArgs e) => Process.Start(((Hyperlink)sender).NavigateUri.AbsoluteUri);

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            loginMessage.Text = "Logging in...";
            new Thread(async () =>
            {
                await Dispatcher.Invoke(async () =>
                {
                    MojangAuthResponse res = await MojangAuth.Authenticate(loginUsername.Text, loginPassword.Text);
                    if (res.IsSuccess)
                    {
                        CurrentSession = res.Session;
                        loginMessage.Text = "Connected!";
                        await Task.WhenAll(updateSkin());
                        toggleLogin();
                    }
                    else Dispatcher.Invoke(() => loginMessage.Text = "Wrong authorization data.");
                });
            }).Start();
        }
        private void loginUsername_ReturnPressed(object sender, KeyEventArgs e) => loginPassword.Focus();
        private void loginPassword_ReturnPressed(object sender, KeyEventArgs e) => Login_Click(sender, null);
    }
}
