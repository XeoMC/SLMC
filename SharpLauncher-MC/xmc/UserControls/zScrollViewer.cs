using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace xmc.uc
{
    public class zScrollViewer : ScrollViewer
    {
        public zScrollViewer() : base()
        {
            CanContentScroll = false;
            AddHandler(MouseWheelEvent, new RoutedEventHandler(ScrollAnimation), true);
        }

        public static readonly DependencyProperty ScrollVerticalOffsetProperty = DependencyProperty.Register("ScrollVerticalOffset", typeof(double), typeof(zScrollViewer), new PropertyMetadata(0d, new PropertyChangedCallback(ScrollVerticalChanged)));
        public static readonly DependencyProperty ScrollHorizontalOffsetProperty = DependencyProperty.Register("ScrollHorizontallOffset", typeof(double), typeof(zScrollViewer), new PropertyMetadata(0d, new PropertyChangedCallback(ScrollHorizontalChanged)));

        private static void ScrollVerticalChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((zScrollViewer)obj).ScrollToVerticalOffset((double)e.NewValue);
        }

        private static void ScrollHorizontalChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((zScrollViewer)obj).ScrollToHorizontalOffset((double)e.NewValue);
        }

        private double nextPositionY = 1;
        private double nextPositionX = 1;

        private void ScrollAnimation(object s, RoutedEventArgs e)
        {
            MouseWheelEventArgs eargs = (MouseWheelEventArgs)e;
            if(true) {
                nextPositionY -= (double)eargs.Delta;
                nextPositionY = Math.Max(nextPositionY, 1);
                nextPositionY = Math.Min(nextPositionY, ScrollableHeight);
                BeginAnimation(ScrollVerticalOffsetProperty, null);
                BeginAnimation(ScrollVerticalOffsetProperty, new DoubleAnimation { From = VerticalOffset, To = nextPositionY, DecelerationRatio = 1, Duration = zGlobals.scrollSpeed });
            } else {
                nextPositionX -= (double)eargs.Delta;
                nextPositionX = Math.Max(nextPositionX, 1);
                nextPositionX = Math.Min(nextPositionX, ScrollableWidth);
                BeginAnimation(ScrollHorizontalOffsetProperty, null);
                BeginAnimation(ScrollHorizontalOffsetProperty, new DoubleAnimation { From = HorizontalOffset, To = nextPositionX, DecelerationRatio = 1, Duration = zGlobals.scrollSpeed });
            }
        }
    }
}