using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace xmc.uc
{
    /// <summary>
    /// Interaction logic for zSwitch.xaml
    /// </summary>
    public partial class zSwitch : UserControl
    {
        public bool isChecked;
        static double ClickDelay = 250;
        public event RoutedEventHandler click;
        public zSwitch()
        {
            InitializeComponent();
            bg.Background = new SolidColorBrush(colorBackground);
            if(isChecked)
                inside.Background = new SolidColorBrush(colorChecked);
            else
                inside.Background = new SolidColorBrush(colorUnchecked);

            this.Loaded += onLoad;
            rect.MouseUp += onMouseUp;
            rect.MouseDown += onMouseDown;
            rect.MouseLeave += onMouseLeave;
            rect.MouseEnter += onMouseEnter;
            timer.Tick += timerTick;
        }

        private void onLoad(object sender, RoutedEventArgs e)
        {
            
            setCheckedInstant(isChecked);
        }
        public DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(ClickDelay) };

        private void onMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsHolding = false;
            timer.Stop();
            if (cl)
            {
                setChecked(!isChecked, true);
                if (click != null)
                click.Invoke(this, e);
            }
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
            if (!allowHolding)
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
                if (isChecked)
                {
                    if (value) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorCheckedClicked, Duration = zGlobals.animationSpeed });
                    else if (IsHovered) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorCheckedHover, Duration = zGlobals.animationSpeed });
                    else inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorChecked, Duration = zGlobals.animationSpeed });
                } else {
                    if (value) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUncheckedClicked, Duration = zGlobals.animationSpeed });
                    else if (IsHovered) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUncheckedHover, Duration = zGlobals.animationSpeed });
                    else inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUnchecked, Duration = zGlobals.animationSpeed });
                }
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
                if (isChecked) {
                    if (IsHolding) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorCheckedClicked, Duration = zGlobals.animationSpeed });
                    else if (value) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorCheckedHover, Duration = zGlobals.animationSpeed });
                    else inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorChecked, Duration = zGlobals.animationSpeed });
                } else {
                    if (IsHolding) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUncheckedClicked, Duration = zGlobals.animationSpeed });
                    else if (value) inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUncheckedHover, Duration = zGlobals.animationSpeed });
                    else inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUnchecked, Duration = zGlobals.animationSpeed });
                }
                MouseHovered = value;
            }
        }
        public static readonly DependencyProperty CornerProperty = DependencyProperty.Register("Corner", typeof(CornerRadius), typeof(zSwitch), new PropertyMetadata(zGlobals.corner));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(zSwitch), new PropertyMetadata("Sample Text"));
        public static readonly DependencyProperty ColorCheckedProperty = DependencyProperty.Register("colorChecked", typeof(Color), typeof(zSwitch), new PropertyMetadata(zGlobals.color));
        public static readonly DependencyProperty ColorCheckedHoverProperty = DependencyProperty.Register("colorCheckedHover", typeof(Color), typeof(zSwitch), new PropertyMetadata(zGlobals.hoverColor));
        public static readonly DependencyProperty ColorCheckedClickedProperty = DependencyProperty.Register("colorCheckedClicked", typeof(Color), typeof(zSwitch), new PropertyMetadata(zGlobals.clickedColor));
        public static readonly DependencyProperty ColorUncheckedProperty = DependencyProperty.Register("colorUnchecked", typeof(Color), typeof(zSwitch), new PropertyMetadata(zGlobals.bgDeepHighlight));
        public static readonly DependencyProperty ColorUncheckedHoverProperty = DependencyProperty.Register("colorUncheckedHover", typeof(Color), typeof(zSwitch), new PropertyMetadata(zGlobals.bgLightHighlight));
        public static readonly DependencyProperty ColorUncheckedClickedProperty = DependencyProperty.Register("colorUncheckedClicked", typeof(Color), typeof(zSwitch), new PropertyMetadata(zGlobals.bgHighlight));
        public static readonly DependencyProperty ColorBackgroundProperty = DependencyProperty.Register("colorBackground", typeof(Color), typeof(zSwitch), new PropertyMetadata(zGlobals.bgDeep));
        public Color colorChecked
        {
            get
            {
                return (Color)GetValue(ColorCheckedProperty);
            }
            set
            {
                SetValue(ColorCheckedProperty, value);
            }
        }
        public Color colorCheckedHover
        {
            get
            {
                return (Color)GetValue(ColorCheckedHoverProperty);
            }
            set
            {
                SetValue(ColorCheckedHoverProperty, value);
            }
        }
        public Color colorCheckedClicked
        {
            get
            {
                return (Color)GetValue(ColorCheckedClickedProperty);
            }
            set
            {
                SetValue(ColorCheckedClickedProperty, value);
            }
        }
        public Color colorUnchecked
        {
            get
            {
                return (Color)GetValue(ColorUncheckedProperty);
            }
            set
            {
                SetValue(ColorUncheckedProperty, value);
            }
        }
        public Color colorUncheckedHover
        {
            get
            {
                return (Color)GetValue(ColorUncheckedHoverProperty);
            }
            set
            {
                SetValue(ColorUncheckedHoverProperty, value);
            }
        }
        public Color colorUncheckedClicked
        {
            get
            {
                return (Color)GetValue(ColorUncheckedClickedProperty);
            }
            set
            {
                SetValue(ColorUncheckedClickedProperty, value);
            }
        }
        public Color colorBackground
        {
            get
            {
                return (Color)GetValue(ColorBackgroundProperty);
            }
            set
            {
                ColorAnimation anim = new ColorAnimation { To = value, Duration = zGlobals.animationSpeed };
                bg.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                SetValue(ColorBackgroundProperty, value);
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
                SetValue(CornerProperty, value);
                inside.CornerRadius = value;
                bg.CornerRadius = value;
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
        public void setCheckedInstant(bool isCheckedNew)
        {
            isChecked = isCheckedNew;
            if (isChecked)
            {
                inside.Margin = new Thickness(inside.ActualWidth / 2 + inside.Margin.Top * 2, inside.Margin.Top, inside.Margin.Top, inside.Margin.Top);
                inside.Background = new SolidColorBrush(colorChecked);
            }
            else
            {
                inside.Margin = new Thickness(inside.Margin.Top);
                inside.Background = new SolidColorBrush(colorUnchecked);
            }
        }
        public void setChecked(bool isCheckedNew, bool hover)
        {
            isChecked = isCheckedNew;
            if (isChecked)
            {
                inside.BeginAnimation(MarginProperty, new ThicknessAnimation { To = new Thickness(inside.ActualWidth / 2 + inside.Margin.Top * 2, inside.Margin.Top, inside.Margin.Top, inside.Margin.Top), Duration = zGlobals.animationSpeed });
                if(hover)
                    inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorCheckedHover, Duration = zGlobals.animationSpeed });
                else
                    inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorChecked, Duration = zGlobals.animationSpeed });
            } else {
                inside.BeginAnimation(MarginProperty, new ThicknessAnimation { To = new Thickness(inside.Margin.Top), Duration = zGlobals.animationSpeed });
                if (hover)
                    inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUncheckedHover, Duration = zGlobals.animationSpeed });
                else
                    inside.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = colorUnchecked, Duration = zGlobals.animationSpeed });
            }
        }
    }
}
