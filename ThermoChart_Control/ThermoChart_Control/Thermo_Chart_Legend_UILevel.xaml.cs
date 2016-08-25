using System.ComponentModel;
using System.Windows.Media;

namespace ThermoChart_Control
{
    /// <summary>
    /// Logique d'interaction pour Thermo_Chart_Legend_UILevel.xaml
    /// </summary>
    internal partial class ThermoChartLegendUiLevel : INotifyPropertyChanged
    {
        #region Constructor

        public ThermoChartLegendUiLevel()
        {
            InitializeComponent();
        }

        #endregion

        #region PropertyChangedEvent

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Property

        private Color _endColor;
        private double _maxValue;
        private double _minValue;
        private Color _startColor;
        private bool _isFirstLevel;
        private bool _isLastLevel;

        public double MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                OnPropertyChanged("MinValue");
            }
        }

        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                OnPropertyChanged("MaxValue");
            }
        }


        public Color StartColor
        {
            get { return _startColor; }
            set
            {
                _startColor = value;
                OnPropertyChanged("StartColor");
            }
        }

        public Color EndColor
        {
            get { return _endColor; }
            set
            {
                _endColor = value;
                OnPropertyChanged("EndColor");
            }
        }


        public bool IsFirstLevel
        {
            get { return _isFirstLevel; }
            set
            {
                _isFirstLevel = value;
                OnPropertyChanged("IsFirstLevel");
            }
        }

        public bool IsLastLevel
        {
            get { return _isLastLevel; }
            set
            {
                _isLastLevel = value;
                OnPropertyChanged("IsLastLevel");
            }
        }

        #endregion
    }
}