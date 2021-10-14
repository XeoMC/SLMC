using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace xmc.uc
{
    /// <summary>
    /// Логика взаимодействия для zEdit.xaml
    /// </summary>
    public partial class zEdit : UserControl
    {
        public zEdit()
        {
            InitializeComponent();
            SetValue(StringsCountProperty, tx.Text.IndexOf('\r') + (tx.Text.Length != 0 ? 1 : 0));
            tx.AcceptsReturn = (bool)GetValue(Multi_Property);
            tx.SelectionBrush = (SolidColorBrush)GetValue(SelectionColorProperty);
            txPass.SelectionBrush = (SolidColorBrush)GetValue(SelectionColorProperty);
            if ((bool)GetValue(Multi_Property))
                tx.TextWrapping = TextWrapping.Wrap;
            else
                tx.TextWrapping = TextWrapping.NoWrap;


            // CUSTOM STYLING
            this.BorderThickness = new Thickness(0, 0, 0, 2);
            this.BorderBrush = new SolidColorBrush(zGlobals.decorationColor);
        }

        private void onLoad(object sender, RoutedEventArgs e)
        {
            gr.Background = Background;
            gr.BorderThickness = BorderThickness;
            gr.BorderBrush = BorderBrush;
            tx.FontFamily = FontFamily;
            tx.FontSize = FontSize;
            txPass.FontFamily = FontFamily;
            txPass.FontSize = FontSize;
            txPass.Password = tx.Text;
            updatePlaceholderVisibility();
            ph.Text = Placeholder;
            if (IsPass)
            {
                txPass.Visibility = Visibility.Visible;
                tx.Visibility = Visibility.Collapsed;
            }
            else
            {
                tx.Visibility = Visibility.Visible;
                txPass.Visibility = Visibility.Collapsed;
            }
        }
        public event EventHandler StringCountChanged;
        public event TextChangedEventHandler TextChanged;
        public event EventHandler ReturnPressed;

        public static readonly DependencyProperty IsPassProperty = DependencyProperty.Register("IsPass", typeof(bool), typeof(zEdit), new PropertyMetadata(false));
        public static readonly DependencyProperty StringsCountProperty = DependencyProperty.Register("StringsCount", typeof(int), typeof(zEdit), new PropertyMetadata(0));
        public static readonly DependencyProperty SelectionColorProperty = DependencyProperty.Register("SelectionColor", typeof(SolidColorBrush), typeof(zEdit), new PropertyMetadata(new SolidColorBrush(zGlobals.color)));
        public static readonly DependencyProperty Multi_Property = DependencyProperty.Register("Multi", typeof(bool), typeof(zEdit), new PropertyMetadata(false));
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(zEdit), new PropertyMetadata(""));
        private void updatePlaceholderVisibility()
        {
            var text = tx.Text;
            if (IsPass) text = txPass.Password;
            if (string.IsNullOrEmpty(text)) ph.Visibility = Visibility.Visible;
            else ph.Visibility = Visibility.Collapsed;
        }

        public string Placeholder
        {
            get
            {
                return (string)GetValue(PlaceholderProperty);
            }
            set
            {
                ph.Text = value;
                updatePlaceholderVisibility();
                SetValue(PlaceholderProperty, value);
            }
        }
        public bool IsPass
        {
            get
            {
                return (bool)GetValue(IsPassProperty);
            }
            set
            {
                SetValue(IsPassProperty, value);
                if (value)
                {
                    txPass.Password = tx.Text;
                    txPass.Visibility = Visibility.Visible;
                    tx.Visibility = Visibility.Collapsed;
                } else
                {
                    tx.Text = txPass.Password;
                    tx.Visibility = Visibility.Visible;
                    txPass.Visibility = Visibility.Collapsed;
                }
            }
        }
        public string Text
        {
            get
            {
                if(IsPass) return txPass.Password;
                return tx.Text;
            }
            set
            {
                if (tx.Text == value && txPass.Password == value) return;
                tx.Text = value;
                txPass.Password= value;
            }
        }
        public SolidColorBrush SelectionColor
        {
            get
            {
                return (SolidColorBrush)GetValue(SelectionColorProperty);
            }
            set
            {
                tx.SelectionBrush = value;
                txPass.SelectionBrush = value;
                SetValue(SelectionColorProperty, value);
            }
        }
        public bool Multi_
        {
            get
            {
                return (bool)GetValue(Multi_Property);
            }
            set
            {
                if (IsPass)
                {
                    SetValue(Multi_Property, value);
                    value = false;
                }
                tx.AcceptsReturn = value;
                if (!value)
                {
                    tx.Text = tx.Text.Replace("\r", "");
                    tx.TextWrapping = TextWrapping.NoWrap;
                }
                else
                    tx.TextWrapping = TextWrapping.Wrap;
                if (IsPass)
                {
                    SetValue(Multi_Property, false);
                    txPass.Password = tx.Text;
                }
            }
        }
        public int StringsCount
        {
            get
            {
                return (int)GetValue(StringsCountProperty);
            }
        }

        private void Tx_TextChanged(object sender, TextChangedEventArgs e)
        {
            int length = tx.Text.IndexOf('\r') + (tx.Text.Length != 0 ? 1 : 0);
            if (length != StringsCount)
            {
                if (Multi_) return;
                SetValue(StringsCountProperty, length);
                if (StringCountChanged != null) StringCountChanged.Invoke(sender, (EventArgs)e);
            }
            else if (ReturnPressed != null) ReturnPressed.Invoke(sender, (EventArgs)e);
            if (TextChanged != null) TextChanged.Invoke(sender, e);
            updatePlaceholderVisibility();
        }
        private void Tx_PassChanged(object sender, RoutedEventArgs e)
        {
            int length = txPass.Password.IndexOf('\r') + (txPass.Password.Length != 0 ? 1 : 0);
            if (length != StringsCount)
            {
                if (Multi_) return;
                SetValue(StringsCountProperty, length);
                if (StringCountChanged != null) StringCountChanged.Invoke(sender, (EventArgs)e);
            }
            else if (ReturnPressed != null) ReturnPressed.Invoke(sender, (EventArgs)e);
            if (TextChanged != null) TextChanged.Invoke(sender, null);
            updatePlaceholderVisibility();
        }
    }
}
