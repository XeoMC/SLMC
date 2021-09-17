
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;

namespace xmc.uc
{
    public partial class zButton : UserControl
    {
        static double ClickDelay = 250;
        public zButton()
        {
            InitializeComponent();
            Path = (Geometry)GetValue(PathProperty);
            ImgSource = (Uri)GetValue(ImageSourceProperty);
            IconWidth = (GridLength)GetValue(IconWidthProperty);
            if (Path != null || ImgSource != null)
                IconWidth = new GridLength(24);
            else
                IconWidth = new GridLength(0);

            this.Loaded += onLoad;
            rect.MouseUp += onMouseUp;
            rect.MouseDown += onMouseDown;
            rect.MouseLeave += onMouseLeave;
            rect.MouseEnter += onMouseEnter;
            timer.Tick += timerTick;
        }
        private void onLoad(object sender, RoutedEventArgs e)
        {
            bg.Color = color;
            border.BorderThickness = BorderThickness;
            border.BorderBrush = BorderBrush;
            border.CornerRadius = Corner;
            tx.FontFamily = FontFamily;
            tx.FontSize = FontSize;
            tx.Foreground = new SolidColorBrush(TextColor);
        }
        private void onMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsHolding = false;
            timer.Stop();
            if (cl && click != null) click.Invoke(this, e);
        }
        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsHolding = true;
            cl = true;
            timer.Start();
        }
        private void onMouseEnter(object sender, MouseEventArgs e)
        {
            IsHovered = true;
        }
        private void onMouseLeave(object sender, MouseEventArgs e)
        {
            IsHovered = false;
            cl = false;
        }

        private void timerTick(object sender, EventArgs e)
        {
            timer.Stop();
            if(!allowHolding)
            cl = false;
        }

        bool allowHolding = true;
        bool cl = false;
        bool ClickHolding = false;
        bool MouseHovered = false;
        
        public bool AllowHolding
        {
            get
            {
                return allowHolding;
            }
            set
            {
                allowHolding = value;
            }
        }

        bool IsHolding
        {
            get
            {
                return ClickHolding;
            }
            set
            {
                if (value) bg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorClicked, Duration = zGlobals.animationSpeed });
                else if (IsHovered) bg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorHover, Duration = zGlobals.animationSpeed });
                else bg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = color, Duration = zGlobals.animationSpeed });
                ClickHolding = value;
            }
        }


        bool IsHovered
        {
            get
            {
                return MouseHovered;
            }
            set
            {
                IsHolding = !value ? false : IsHolding;
                if (IsHolding) bg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorClicked, Duration = zGlobals.animationSpeed });
                else if(value) bg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorHover, Duration = zGlobals.animationSpeed });
                else bg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = color, Duration = zGlobals.animationSpeed });
                MouseHovered = value;
            }
        }

        public DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(ClickDelay) };

        public Color color
        {
            get
            {
                return (Color)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
                bg.Color = value;
            }
        }
        public Color colorHover
        {
            get
            {
                return (Color)GetValue(ColorHoverProperty);
            }
            set
            {
                SetValue(ColorHoverProperty, value);
            }
        }
        public Color colorClicked
        {
            get
            {
                return (Color)GetValue(ColorClickedProperty);
            }
            set
            {
                SetValue(ColorClickedProperty, value);
            }
        }
        public Color colorDisabled
        {
            get
            {
                return (Color)GetValue(ColorDisabledProperty);
            }
            set
            {
                SetValue(ColorDisabledProperty, value);
            }
        }

        public event RoutedEventHandler click;
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(zButton), new PropertyMetadata("Sample Text"));
        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(zButton), new PropertyMetadata(zGlobals.font));
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(zButton), new PropertyMetadata(zGlobals.fontSize));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(Uri), typeof(zButton));
        public static readonly DependencyProperty CornerProperty = DependencyProperty.Register("Corner", typeof(CornerRadius), typeof(zButton), new PropertyMetadata(zGlobals.corner));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("color", typeof(Color), typeof(zButton), new PropertyMetadata(zGlobals.color));
        public static readonly DependencyProperty ColorHoverProperty = DependencyProperty.Register("colorHover", typeof(Color), typeof(zButton), new PropertyMetadata(zGlobals.hoverColor));
        public static readonly DependencyProperty ColorClickedProperty = DependencyProperty.Register("colorClicked", typeof(Color), typeof(zButton), new PropertyMetadata(zGlobals.clickedColor));
        public static readonly DependencyProperty ColorDisabledProperty = DependencyProperty.Register("colorDisabled", typeof(Color), typeof(zButton), new PropertyMetadata(zGlobals.disabledColor));
        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register("TextColor", typeof(Color), typeof(zButton), new PropertyMetadata(zGlobals.textColor));
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(Geometry), typeof(zButton));
        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register("IconWidth", typeof(GridLength), typeof(zButton));

        public GridLength IconWidth
        {
            get
            {
                return (GridLength)GetValue(IconWidthProperty);
            }
            set
            {
                w.Width = value;
                SetValue(IconWidthProperty, value);
            }
        }
        public Geometry Path
        {
            get
            {
                return (Geometry)GetValue(PathProperty);
            }
            set
            {
                if (value != null)
                    w.Width = new GridLength(24);
                else
                    w.Width = new GridLength(0);
                SetValue(PathProperty, value);
            }
        }
        public CornerRadius Corner
        {
            get
            {
                return (CornerRadius)GetValue(CornerProperty);
            }
            set
            {
                border.CornerRadius = value;
                SetValue(CornerProperty, value);
            }
        }
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }
        public double FontSize
        {
            get
            {
                return (double)GetValue(FontSizeProperty);
            }
            set
            {
                SetValue(FontSizeProperty, value);
            }
        }
        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)GetValue(FontFamilyProperty);
            }
            set
            {
                SetValue(FontFamilyProperty, value);
            }
        }
        public Uri ImgSource
        {
            get
            {
                return (Uri)GetValue(ImageSourceProperty);
            }
            set
            {
                if (value != null)
                {
                    IconWidth = new GridLength(this.ActualHeight);
                    img.Source = new BitmapImage(value);
                }
                else
                    IconWidth = new GridLength(0);

                SetValue(ImageSourceProperty, value);
            }
        }
        public Color TextColor
        {
            get
            {
                return (Color)GetValue(TextColorProperty);
            }
            set
            {
                tx.Foreground = new SolidColorBrush(value);
                SetValue(TextColorProperty, value);
            }
        }
    }
}
