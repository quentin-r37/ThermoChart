using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ThermoChart_Control
{
    internal class ThermoChartUiLevelContainer : DockPanel
    {
        #region DependancyProperties

        public static readonly DependencyProperty UiLevelSourceProperty =
            DependencyProperty.Register("UiLevelSource", typeof (IEnumerable), typeof (ThermoChartUiLevelContainer),
                                        new PropertyMetadata(UiLevelSourceChanged));

        public IEnumerable UiLevelSource
        {
            get { return (IEnumerable) GetValue(UiLevelSourceProperty); }
            set { SetValue(UiLevelSourceProperty, value); }
        }

        private static void UiLevelSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var container = (ThermoChartUiLevelContainer) o;
            container.Children.Clear();
            if (e.NewValue == e.OldValue) return;
            if (e.NewValue != null)
            {
                foreach (ThermoChartLegendUiLevel item in (List<ThermoChartLegendUiLevel>) e.NewValue)
                {
                    item.SetValue(DockProperty, Dock.Top);
                    container.Children.Add(item);
                }
            }
        }

        #endregion

        public ThermoChartUiLevelContainer()
        {
            var transformCollection = new TransformCollection {new RotateTransform(180), new TranslateTransform(0, 0)};
            var transformGroup = new TransformGroup {Children = transformCollection};

            RenderTransform = transformGroup;
            RenderTransformOrigin = new Point(0.5, 0.5);

            UseLayoutRounding = true;
            LastChildFill = false;
        }
    }
}