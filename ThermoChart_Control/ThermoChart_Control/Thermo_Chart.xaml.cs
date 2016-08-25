using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ThermoChart_Control
{
    /// <summary>
    /// Logique d'interaction pour Thermo_Chart.xaml
    /// </summary>
    partial class ThermoChart
    {
        #region PublicProperty

        public ThermoChartStatus ChartStatus = new ThermoChartStatus();
        public Dictionary<double, ThermoChartLevel> LevelsDictionnary = new Dictionary<double, ThermoChartLevel>();

        #endregion

        #region PublicStaticProperty

        #endregion

        #region DependancyProperty

        public static readonly DependencyProperty MaxGlobalValProperty =
            DependencyProperty.Register("MaxGlobalVal", typeof(double), typeof(ThermoChart),
                                        new PropertyMetadata(MaxGlobalValChanged));

        public static readonly DependencyProperty MinGlobalValProperty =
            DependencyProperty.Register("MinGlobalVal", typeof(double), typeof(ThermoChart),
                                        new PropertyMetadata(MinGlobalValChanged));

        public static readonly DependencyProperty LegendTitleProperty =
            DependencyProperty.Register("LegendTitle", typeof(string), typeof(ThermoChart),
                                        new PropertyMetadata(LegendTitleChanged));

        public static readonly DependencyProperty LevelsSourceProperty =
            DependencyProperty.Register("LevelsSource", typeof(List<ThermoChartLevel>), typeof(ThermoChart),
                                        new PropertyMetadata(LevelsSourceChanged));

        public static readonly DependencyProperty DataPointsSourceProperty =
            DependencyProperty.Register("DataPointsSource", typeof(List<ThermoDataPoint>), typeof(ThermoChart),
                                        new PropertyMetadata(DataPointsSourceChanged));

        public static readonly DependencyProperty AxisXTextProperty =
            DependencyProperty.Register("AxisXText", typeof(string), typeof(ThermoChart),
                                        new PropertyMetadata(AxisXTextChanged));

        public static readonly DependencyProperty AxisYTextProperty =
            DependencyProperty.Register("AxisYText", typeof(string), typeof(ThermoChart),
                                        new PropertyMetadata(AxisYTextChanged));

        public static readonly DependencyProperty AxisXMaxWidthProperty =
            DependencyProperty.Register("AxisXMaxWidth", typeof(double), typeof(ThermoChart),
                                        new PropertyMetadata(AxisXMaxWidthChanged));

        public static readonly DependencyProperty SmoothLevelProperty =
            DependencyProperty.Register("SmoothLevel", typeof(double), typeof(ThermoChart),
                                        new PropertyMetadata(SmoothLevelChanged));

        public static readonly DependencyProperty EnableUserInteractionProperty =
            DependencyProperty.Register("EnableUserInteraction", typeof(bool), typeof(ThermoChart),
                                        new PropertyMetadata(EnableUserInteractionChanged));

        public double MaxGlobalVal
        {
            get { return (double)GetValue(MaxGlobalValProperty); }
            set { SetValue(MaxGlobalValProperty, value); }
        }

        public double MinGlobalVal
        {
            get { return (double)GetValue(MinGlobalValProperty); }
            set { SetValue(MinGlobalValProperty, value); }
        }

        public string LegendTitle
        {
            get { return (string)GetValue(LegendTitleProperty); }
            set { SetValue(LegendTitleProperty, value); }
        }

        public List<ThermoChartLevel> LevelsSource
        {
            get { return (List<ThermoChartLevel>)GetValue(LevelsSourceProperty); }
            set { SetValue(LevelsSourceProperty, value); }
        }

        public List<ThermoDataPoint> DataPointsSource
        {
            get { return (List<ThermoDataPoint>)GetValue(DataPointsSourceProperty); }
            set { SetValue(DataPointsSourceProperty, value); }
        }

        public string AxisXText
        {
            get { return (string)GetValue(AxisXTextProperty); }
            set { SetValue(AxisXTextProperty, value); }
        }

        public string AxisYText
        {
            get { return (string)GetValue(AxisYTextProperty); }
            set { SetValue(AxisYTextProperty, value); }
        }

        public double AxisXMaxWidth
        {
            get { return (double)GetValue(AxisXMaxWidthProperty); }
            set { SetValue(AxisXMaxWidthProperty, value); }
        }

        public double SmoothLevel
        {
            get { return (double)GetValue(SmoothLevelProperty); }
            set { SetValue(SmoothLevelProperty, value); }
        }

        public bool EnableUserInteraction
        {
            get { return (bool)GetValue(EnableUserInteractionProperty); }
            set { SetValue(EnableUserInteractionProperty, value); }
        }

        #endregion

        #region EventDependancyProperty

        private static void MinGlobalValChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var thermo = (ThermoChart)o;
            if ((double)e.NewValue != 0)
            {
                if ((double)e.NewValue != (double)e.OldValue)
                {
                    thermo.Legend.MinGlobalVal = (double)e.NewValue;
                }
            }
        }

        private static void MaxGlobalValChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var thermo = (ThermoChart)o;
            if ((double)e.NewValue != 0)
            {
                if ((double)e.NewValue != (double)e.OldValue)
                {
                    thermo.Legend.MaxGlobalVal = (double)e.NewValue;
                }
            }
        }

        private static void LevelsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var thermo = (ThermoChart)o;
            List<ThermoChartLevel> lvls = ((IEnumerable<ThermoChartLevel>)e.NewValue).ToList();
            thermo.LevelsSourceChangedEvent(lvls);
        }

        public void LevelsSourceChangedEvent(List<ThermoChartLevel> lvls)
        {
            #region Status

            if (lvls != null)
            {
                ChartStatus.LevelStatus = ThermoChartEnumStatus.Valid;
            }
            else
            {
                ChartStatus.LevelStatus = ThermoChartEnumStatus.Invalid;
                Legend.ShowError();
            }

            #endregion

            LevelsDictionnary.Clear();
            foreach (ThermoChartLevel t in lvls)
            {
                LevelsDictionnary[t.MinValue] = t;
            }
            LevelsDictionnary[lvls[lvls.Count - 1].MaxValue] = new ThermoChartLevel(lvls[lvls.Count - 1].StartColor,
                                                                                       lvls[lvls.Count - 1].EndColor,
                                                                                       lvls[lvls.Count - 1].MinValue,
                                                                                       lvls[lvls.Count - 1].MaxValue);
        }

        private static void LegendTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var thermo = (ThermoChart)o;
            if (!string.IsNullOrEmpty((string)e.NewValue))
            {
                if ((string)e.NewValue != (string)e.OldValue)
                {
                    thermo.Legend.LegendTitle = (string)e.NewValue;
                }
            }
        }

        private static void DataPointsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var thermo = (ThermoChart)o;
            List<ThermoDataPoint> zdatas = ((IEnumerable<ThermoDataPoint>)e.NewValue).ToList();
            thermo.DataPointsSourceChangedEvent(zdatas);
        }

        private void DataPointsSourceChangedEvent(List<ThermoDataPoint> zdatas)
        {
            if (zdatas != null)
            {
                if (zdatas.Count > 0)
                {
                    ChartStatus.DatasStatus = ThermoChartEnumStatus.Valid;
                }
            }
            else
            {
                ChartStatus.DatasStatus = ThermoChartEnumStatus.Invalid;
            }
            ChartArea.DataSource = zdatas;
        }

        private static void AxisYTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var thermo = (ThermoChart)o;
            if (!string.IsNullOrEmpty((string)e.NewValue))
            {
                if ((string)e.NewValue != (string)e.OldValue)
                {
                    thermo.ChartArea.AxisYtext = (string)e.NewValue;
                }
            }
        }

        private static void AxisXTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var thermo = (ThermoChart)o;
            if (!string.IsNullOrEmpty((string)e.NewValue))
            {
                if ((string)e.NewValue != (string)e.OldValue)
                {
                    thermo.ChartArea.AxisXtext = (string)e.NewValue;
                }
            }
        }

        private static void AxisXMaxWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var thermo = (ThermoChart)o;
            if ((double)e.NewValue != 0)
            {
                if ((double)e.NewValue != (double)e.OldValue)
                {
                    thermo.ChartArea.WidthXLabel = (double)e.NewValue;
                }
            }
        }

        private static void SmoothLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var thermo = (ThermoChart)o;

            if ((double)e.NewValue != (double)e.OldValue)
            {
                thermo.ChartArea.SmoothLevel = (double)e.NewValue;
            }
        }

        private static void EnableUserInteractionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var thermo = (ThermoChart)o;

            if ((bool)e.NewValue != (bool)e.OldValue)
            {
                thermo.ChartArea.EnableUserInteraction = (bool)e.NewValue;
            }
        }

        #endregion

        #region PrivateProperty

        private readonly DispatcherTimer _resetClipTimer = new DispatcherTimer
        { Interval = TimeSpan.FromMilliseconds(1), IsEnabled = false };

        #endregion

        #region Constructor

        public ThermoChart()
        {
            InitializeComponent();
            Loaded += Thermo_Chart_Loaded;
            SizeChanged += Thermo_Chart_SizeChanged;
            //_resetClipTimer.Tick += new EventHandler(_resetClipTimer_Tick);
        }

        #endregion

        #region PrivateMethod

        private void Thermo_Chart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshLegendLevel();
        }

        private void Thermo_Chart_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshLegendLevel();
        }

        private void RefreshLegendLevel()
        {
            var tmpList = new List<ThermoChartLegendUiLevel>();
            if (LevelsSource != null)
            {
                for (int i = 0; i < LevelsSource.Count; i++)
                {
                    var th = new ThermoChartLegendUiLevel
                    {
                        StartColor = LevelsSource[i].StartColor,
                        EndColor = LevelsSource[i].EndColor,
                        MinValue = LevelsSource[i].MinValue,
                        MaxValue = LevelsSource[i].MaxValue
                    };


                    double ecartLoc = th.MaxValue - th.MinValue;
                    //double ecartLoc = th.MinValue - th.MaxValue;
                    double ecartGlob = Legend.MaxGlobalVal - Legend.MinGlobalVal;
                    if (ecartGlob != 0)
                    {
                        double ratio = ecartLoc / ecartGlob;
                        th.Height = Legend.StackLevel.ActualHeight*ratio-0.5;

                        if (i == LevelsSource.Count - 1)
                        {
                            th.IsLastLevel = true;
                        }
                        else if (i == 0)
                        {
                            th.IsFirstLevel = true;
                        }
                        else
                        {
                            th.IsFirstLevel = false;
                            th.IsLastLevel = false;
                        }
                        tmpList.Add(th);
                    }
                    else
                    {
                        ChartStatus.LevelStatus = ThermoChartEnumStatus.Invalid;
                    }
                }
            }
            Legend.ShowError();
            if (ChartStatus.LevelStatus == ThermoChartEnumStatus.Valid)
            {
                Legend.ListPalier = tmpList;
            }
        }

        #endregion

        #region PublicMethod

        public void RefreshLegend()
        {
            RefreshLegendLevel();
        }

        public void RefreshAxisXAxisY()
        {
            ChartArea.DrawDivisionsX();
            ChartArea.DrawDivisionsY();

            _resetClipTimer.IsEnabled = true;
            _resetClipTimer.Stop();
            _resetClipTimer.Start();
        }

        public void RefreshChartArea()
        {
            var pts = DataPointsSource;
            ChartArea.DataSource = pts;
            ChartArea.DrawDivisionsX();
            ChartArea.DrawDivisionsY();
            ChartArea.DrawChartArea();
        }

        public void ResetClip()
        {
            ChartArea.ResetClip();
        }

        public void GetAvgMaxMinColorForSelectedBlocks(out double avg, out double max, out double min,
                                                       out Brush[] colors)
        {
            ChartArea.GetAvgMaxMinColorForSelectedBlocks(out avg, out max, out min, out colors);
        }

        #endregion

        #region PublicEvent

        public static readonly RoutedEvent SelectedZoneChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedZoneChangedEventHandler", RoutingStrategy.Bubble,
                                             typeof(RoutedEventHandler), typeof(ThermoChartArea));

        public event RoutedEventHandler SelectedZoneChangedEventHandler
        {
            add { AddHandler(SelectedZoneChangedEvent, value); }
            remove { RemoveHandler(SelectedZoneChangedEvent, value); }
        }

        public void _resetClipTimer_Tick(object sender, EventArgs e)
        {
            if (SmoothLevel > 0)
            {
                ResetClip();
            }
        }

        #endregion
    }
}