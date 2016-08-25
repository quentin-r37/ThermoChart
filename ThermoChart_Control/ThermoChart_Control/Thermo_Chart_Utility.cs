using System;
using System.Windows;
using System.Windows.Media;

namespace ThermoChart_Control
{
    public static class ThermoChartUtility
    {
        #region PublicStaticMethod

        public static Window GetTopParent(UIElement el)
        {
            DependencyObject dpParent = el;
            do
            {
                dpParent = LogicalTreeHelper.GetParent(dpParent);
            } while (dpParent.GetType().BaseType != typeof (Window));

            return dpParent as Window;
        }

        public static ThermoChart GetCurrentChart(FrameworkElement element)
        {
            do
            {
                element = (FrameworkElement) element.Parent;
            } while (element.GetType() != typeof (ThermoChart));

            return element as ThermoChart;
        }

        public static Color GetRandomColor()
        {
            var rnd = new Random();
            return new Color {R = (byte) rnd.Next(255), G = (byte) rnd.Next(255), B = (byte) rnd.Next(255)};
        }

        #endregion
    }
}