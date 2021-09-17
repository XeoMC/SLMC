using System;
using System.Windows;
using System.Windows.Media;

namespace xmc.uc
{
    public static class zGlobals
    {
        public static Color color = ((SolidColorBrush)Application.Current.Resources["zColor"]).Color;
        public static Color hoverColor = ((SolidColorBrush)Application.Current.Resources["zHoverColor"]).Color;
        public static Color clickedColor = ((SolidColorBrush)Application.Current.Resources["zClickedColor"]).Color;
        public static Color disabledColor = ((SolidColorBrush)Application.Current.Resources["zDisabledColor"]).Color;
        public static Color textColor = ((SolidColorBrush)Application.Current.Resources["zTextColor"]).Color;
        public static Color decorationColor = ((SolidColorBrush)Application.Current.Resources["zDecorationColor"]).Color;

        public static Color bgMain = ((SolidColorBrush)Application.Current.Resources["zBackground"]).Color;
        public static Color bgHighlight = ((SolidColorBrush)Application.Current.Resources["zBackgroundHighlight"]).Color;
        public static Color bgDeep = ((SolidColorBrush)Application.Current.Resources["zBackgroundDeep"]).Color;
        public static Color bgDeepHighlight = ((SolidColorBrush)Application.Current.Resources["zBackgroundDeepHighlight"]).Color;
        public static Color bgLight = ((SolidColorBrush)Application.Current.Resources["zBackgroundLight"]).Color;
        public static Color bgLightHighlight = ((SolidColorBrush)Application.Current.Resources["zBackgroundLightHighlight"]).Color;

        public static FontFamily font = (FontFamily)Application.Current.Resources["zFontFamily"];
        public static double fontSize = (double)Application.Current.Resources["zFontSize"];

        public static CornerRadius corner = (CornerRadius)Application.Current.Resources["zCorner"];

        public static TimeSpan animationSpeed = TimeSpan.FromMilliseconds(100);
        public static TimeSpan scrollSpeed = animationSpeed.Multiply(2);
    }
}
