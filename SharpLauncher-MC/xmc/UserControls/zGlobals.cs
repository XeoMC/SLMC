using System;
using System.Windows;
using System.Windows.Media;
using SharpLauncher_MC;

namespace xmc.uc
{
    public static class zGlobals
    {
        public static Color color = ((SolidColorBrush)App.Current.Resources["zColor"]).Color;
        public static Color hoverColor = ((SolidColorBrush)App.Current.Resources["zHoverColor"]).Color;
        public static Color clickedColor = ((SolidColorBrush)App.Current.Resources["zClickedColor"]).Color;
        public static Color disabledColor = ((SolidColorBrush)App.Current.Resources["zDisabledColor"]).Color;
        public static Color textColor = ((SolidColorBrush)App.Current.Resources["zTextColor"]).Color;
        public static Color decorationColor = ((SolidColorBrush)App.Current.Resources["zDecorationColor"]).Color;

        public static Color bgMain = ((SolidColorBrush)App.Current.Resources["zBackground"]).Color;
        public static Color bgHighlight = ((SolidColorBrush)App.Current.Resources["zBackgroundHighlight"]).Color;
        public static Color bgDeep = ((SolidColorBrush)App.Current.Resources["zBackgroundDeep"]).Color;
        public static Color bgDeepHighlight = ((SolidColorBrush)App.Current.Resources["zBackgroundDeepHighlight"]).Color;
        public static Color bgLight = ((SolidColorBrush)App.Current.Resources["zBackgroundLight"]).Color;
        public static Color bgLightHighlight = ((SolidColorBrush)App.Current.Resources["zBackgroundLightHighlight"]).Color;

        public static FontFamily font = (FontFamily)App.Current.Resources["zFontFamily"];
        public static double fontSize = (double)App.Current.Resources["zFontSize"];

        public static CornerRadius corner = (CornerRadius)App.Current.Resources["zCorner"];

        public static TimeSpan animationSpeed = TimeSpan.FromMilliseconds(100);
        public static TimeSpan scrollSpeed = animationSpeed.Multiply(2);
    }
}
