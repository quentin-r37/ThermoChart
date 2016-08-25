using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ThermoChart_Control
{
    /// <summary>
    /// Logique d'interaction pour Thermo_Chart_Legend.xaml
    /// </summary>
    internal partial class ThermoChartLegend : INotifyPropertyChanged
    {
        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Property

        private string _legendTitle = "titre";
        private List<ThermoChartLegendUiLevel> _listPalier = new List<ThermoChartLegendUiLevel>();

        private double _maxGlobalVal;

        private double _minGlobalVal;

        public List<ThermoChartLegendUiLevel> ListPalier
        {
            get { return _listPalier; }
            set
            {
                _listPalier = value;
                OnPropertyChanged("ListPalier");
            }
        }

        public double MaxGlobalVal
        {
            get { return _maxGlobalVal; }
            set
            {
                _maxGlobalVal = value;
                OnPropertyChanged("MaxGlobalVal");
            }
        }

        public double MinGlobalVal
        {
            get { return _minGlobalVal; }
            set
            {
                _minGlobalVal = value;
                OnPropertyChanged("MinGlobalVal");
            }
        }

        public string LegendTitle
        {
            get { return _legendTitle; }
            set
            {
                _legendTitle = value;
                OnPropertyChanged("LegendTitle");
            }
        }

        #endregion

        #region Constructor

        public ThermoChartLegend()
        {
            InitializeComponent();
        }

        #endregion

        #region PrivateMethod

        public void ShowError()
        {
            if ((GetCurrentChart()).ChartStatus.LevelStatus == ThermoChartEnumStatus.Valid)
            {
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    StackLevel.Children.Clear();
                    var panel = new StackPanel {VerticalAlignment = VerticalAlignment.Center};
                    panel.Children.Add(new TextBlock
                    {
                        Foreground = Brushes.Red,
                        FontWeight = FontWeights.Bold,
                        Text = "Paliers Invalides",
                        HorizontalAlignment = HorizontalAlignment.Center
                    });
                    StackLevel.Children.Add(panel);
                }, DispatcherPriority.Render);
            }
        }

        private ThermoChart GetCurrentChart()
        {
            FrameworkElement element = this;
            do
            {
                element = (FrameworkElement) element.Parent;
            } while (element.GetType() != typeof (ThermoChart));

            return element as ThermoChart;
        }

        #endregion
    }
}