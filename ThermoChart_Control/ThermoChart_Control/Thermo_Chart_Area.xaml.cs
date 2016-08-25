using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ThermoChart_Control
{
    /// <summary>
    /// Logique d'interaction pour Thermo_Chart_Area.xaml
    /// </summary>
    internal partial class ThermoChartArea : INotifyPropertyChanged
    {
        #region PropertyChangedEvent

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region PublicProperty

        public GridLength MaxWidthYLabel
        {
            get { return _maxWidthYLabel; }
            set
            {
                _maxWidthYLabel = value;
                OnPropertyChanged("MaxWidthYLabel");
            }
        }

        public GridLength MaxWidthXLabel
        {
            get { return _maxWidthXLabel; }
            set
            {
                _maxWidthXLabel = value;
                OnPropertyChanged("MaxWidthXLabel");
            }
        }

        public List<ThermoDataPoint> DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                if (value != null)
                {
                    _dataSource = _dataSource.GroupBy(arret => arret.X).
                        SelectMany(grp => grp).ToList();

                    var hourList = new List<string>();
                    foreach (ThermoDataPoint tdp in _dataSource)
                    {
                        if (hourList.Contains(tdp.Y) == false)
                        {
                            hourList.Add(tdp.Y);
                        }
                    }

                    var listTmpPoint = new List<ThermoDataPoint>();
                    for (int i = 0; i < hourList.Count; i++)
                    {
                        if (_dataSource[i].X == _dataSource[i + 1].X && _dataSource[i].Y == _dataSource[i + 1].Y)
                        {
                            listTmpPoint.Add(_dataSource[i + 1]);
                            _dataSource.RemoveAt(i + 1);
                        }
                    }
                    foreach (ThermoDataPoint tdp in listTmpPoint)
                    {
                        _dataSource.Add(tdp);
                    }
                    int countPlage = _dataSource.Count(arr => arr.X == _dataSource[0].X);
                    _ydivisions = countPlage - listTmpPoint.Count;
                    int countWpt = _dataSource.Count(arr => arr.Y == _dataSource[0].Y);
                    _xdivisions = countWpt;
                    Dispatcher.Invoke(() =>
                    {
                        GraphArea.Children.Clear();
                        GraphArea.ColumnDefinitions.Clear();
                        GraphArea.RowDefinitions.Clear();
                        ODivisionsX.Children.Clear();
                        ODivisionsY.Children.Clear();

                        var panel = new StackPanel {Width = 120, Height = 50};
                        panel.Children.Add(new ProgressBar {IsIndeterminate = true, Width = 120, Height = 23});
                        panel.Children.Add(new TextBlock
                        {Text = "Loading...", HorizontalAlignment = HorizontalAlignment.Center});
                        GraphArea.Children.Add(panel);
                    });
                    _resizeTimer.IsEnabled = true;
                    _resizeTimer.Stop();
                    _resizeTimer.Start();
                }
                OnPropertyChanged("DataSource");
            }
        }

        public string AxisXtext
        {
            get { return _axisXtext; }
            set
            {
                _axisXtext = value;
                OnPropertyChanged("AxisXtext");
            }
        }

        public string AxisYtext
        {
            get { return _axisYtext; }
            set
            {
                _axisYtext = value;
                OnPropertyChanged("AxisYtext");
            }
        }

        public double WidthXLabel
        {
            get { return _widthXLabel; }
            set
            {
                _widthXLabel = value;
                MaxWidthXLabel = new GridLength(value + 25);
                if (_smoothLevel > 0)
                {
                    ResetClip();
                }
                else
                {
                    GraphArea.Clip = null;
                }
                OnPropertyChanged("WidthXLabel");
            }
        }

        public double SmoothLevel
        {
            get { return _smoothLevel; }
            set
            {
                _smoothLevel = value;
                OnPropertyChanged("SmoothLevel");
            }
        }

        public bool EnableUserInteraction
        {
            get { return _enableUserInteraction; }
            set
            {
                _enableUserInteraction = value;
                OnPropertyChanged("EnableUserInteraction");
            }
        }

        #endregion

        #region PrivateProperties

        private readonly List<ThermoChartColorBlock> _thermoChartColorBlockList = new List<ThermoChartColorBlock>();
        private readonly Label _labelAxisX = new Label {FontSize = 14, FontWeight = FontWeights.Bold};
        private readonly Label _labelAxisY = new Label {FontSize = 14, FontWeight = FontWeights.Bold};

        private readonly DispatcherTimer _resizeTimer = new DispatcherTimer
            {Interval = new TimeSpan(0, 0, 0, 0, 0), IsEnabled = false};

        private List<ThermoChartColorBlock> _thermoChartColorBlockSelectedList = new List<ThermoChartColorBlock>();
        private string _axisXtext = "axisX";
        private string _axisYtext = "axisY";

        private List<ThermoDataPoint> _dataSource = new List<ThermoDataPoint>();
        private GridLength _maxWidthXLabel = new GridLength(100);
        private GridLength _maxWidthYLabel = new GridLength(10);
        private double _widthXLabel = -1;
        private int _xdivisions = 2;
        private int _ydivisions = 1;
        private bool _enableUserInteraction;
        private bool _firstLoad = true;
        private int _leveldef = 1;
        private Point _ptEnd = new Point(0, 0);
        private Point _ptStart = new Point(0, 0);
        private Random _rnd = new Random();
        private double _smoothLevel;

        #endregion

        #region Constructor

        public ThermoChartArea()
        {
            InitializeComponent();
            Loaded += Thermo_Chart_Area_Loaded;
            SizeChanged += Thermo_Chart_Area_SizeChanged;
            GraphArea.MouseMove += GraphArea_MouseMove;
            GraphArea.MouseDown += GraphArea_MouseDown;
            _resizeTimer.Tick += _resizeTimer_Tick;
            GraphArea.SnapsToDevicePixels = true;
            //    GraphArea.UseLayoutRounding = false;
        }

        #endregion

        #region Event

        private void GraphArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (ThermoChartColorBlock item in _thermoChartColorBlockList)
            {
                item.IsHover = false;
            }
            if (EnableUserInteraction && Math.Abs(_smoothLevel) < double.Epsilon)
            {
                _ptStart.X = ((ThermoChartColorBlock) e.Source).XPos;
                _ptStart.Y = ((ThermoChartColorBlock) e.Source).YPos;
            }
        }

        private void GraphArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (EnableUserInteraction && Math.Abs(_smoothLevel) < double.Epsilon)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _ptEnd.X = ((ThermoChartColorBlock) e.Source).XPos;
                    _ptEnd.Y = ((ThermoChartColorBlock) e.Source).YPos;

                    foreach (ThermoChartColorBlock item in _thermoChartColorBlockList)
                    {
                        item.IsHover = false;
                    }
                    ((ThermoChartColorBlock) e.Source).IsTopLeftSelected = true;

                    List<ThermoChartColorBlock> tmp;
                    if (_ptStart.X >= _ptEnd.X && _ptStart.Y >= _ptEnd.Y)
                    {
                        tmp = GetThermoChartColorBlockBetweenTwoPoints(_ptEnd, _ptStart);
                    }
                    else
                    {
                        tmp = GetThermoChartColorBlockBetweenTwoPoints(_ptStart, _ptEnd);
                    }

                    _thermoChartColorBlockSelectedList = tmp;
                    if (_thermoChartColorBlockSelectedList.Count > 0)
                    {
                        var newEventArgs = new RoutedEventArgs(ThermoChart.SelectedZoneChangedEvent);
                        RaiseEvent(newEventArgs);
                    }
                }
            }
        }

        #endregion

        #region Constante

        private const double GRAPH_BORDER = 80;

        #endregion

        #region PrivateEvent Size Load

        private void Thermo_Chart_Area_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _resizeTimer.Interval = TimeSpan.FromSeconds(_firstLoad ? 0 : 1);


            Dispatcher.Invoke(() =>
            {
                GraphArea.Children.Clear();
                GraphArea.ColumnDefinitions.Clear();
                GraphArea.RowDefinitions.Clear();
                ODivisionsX.Children.Clear();
                ODivisionsY.Children.Clear();
            }, DispatcherPriority.Render);


            _resizeTimer.IsEnabled = true;
            _resizeTimer.Stop();
            _resizeTimer.Start();
            PanelLoad.Visibility = Visibility.Visible;
        }

        private void _resizeTimer_Tick(object sender, EventArgs e)
        {
            if ((bool) (DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof (DependencyObject)).DefaultValue))
                return;
            PanelLoad.Visibility = Visibility.Hidden;

            GraphArea.Clip = _smoothLevel > 0 ? new RectangleGeometry(new Rect(0, 0, GraphArea.ActualWidth, GraphArea.ActualHeight)) : null;
            if ((ThermoChartUtility.GetCurrentChart(this)).ChartStatus.DatasStatus == ThermoChartEnumStatus.Valid)
            {
                _resizeTimer.IsEnabled = false;
                GraphArea.Visibility = Visibility.Visible;
                ClearDataArea();
                DrawChartArea();
                ClearAxisX();
                DrawDivisionsX();
                ClearAxisY();
                DrawDivisionsY();
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    GraphArea.Children.Clear();
                    GraphArea.ColumnDefinitions.Clear();
                    GraphArea.RowDefinitions.Clear();
                    ODivisionsX.Children.Clear();
                    ODivisionsY.Children.Clear();

                    var panel = new StackPanel {Width = 120, Height = 50};
                    panel.Children.Add(new TextBlock
                    {
                        Foreground = Brushes.Red,
                        FontWeight = FontWeights.Bold,
                        Text = "Données Invalides",
                        HorizontalAlignment = HorizontalAlignment.Center
                    });
                    GraphArea.Children.Add(panel);
                }, DispatcherPriority.Render);
            }
        }

        private void Thermo_Chart_Area_Loaded(object sender, RoutedEventArgs e)
        {
            if ((bool) (DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof (DependencyObject)).DefaultValue))
                return;

            var pnl = new StackPanel {Width = 120, Height = 50};
            pnl.Children.Add(new ProgressBar {IsIndeterminate = true, Width = 120, Height = 23});
            pnl.Children.Add(new TextBlock {Text = "Loading...", HorizontalAlignment = HorizontalAlignment.Center});
            PanelLoad.Children.Add(pnl);


            if ((ThermoChartUtility.GetCurrentChart(this)).ChartStatus.DatasStatus == ThermoChartEnumStatus.Valid)
            {
                ClearAxisX();
                DrawDivisionsX();
                ClearAxisY();
                DrawDivisionsY();
                ClearDataArea();
                DrawChartArea();
                _firstLoad = false;
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    GraphArea.Children.Clear();
                    GraphArea.ColumnDefinitions.Clear();
                    GraphArea.RowDefinitions.Clear();
                    ODivisionsX.Children.Clear();
                    ODivisionsY.Children.Clear();
                    var panel = new StackPanel {Width = 120, Height = 50};
                    panel.Children.Add(new TextBlock
                    {
                        Foreground = Brushes.Red,
                        FontWeight = FontWeights.Bold,
                        Text = "Données Invalides",
                        HorizontalAlignment = HorizontalAlignment.Center
                    });
                    GraphArea.Children.Add(panel);
                }, DispatcherPriority.Render);
                _firstLoad = false;
            }
        }

        #endregion

        #region PrivateProperty

        //private double _yoriginvalue = 0.0;
        //private double _yfullscalevalue = 100.0;
        //private double _xoriginvalue = 0.0;
        //private double _xfullscalevalue = 10000.0;
        private readonly Random _r = new Random();
        private Line[] _oLineX;
        private Line[] _oLineY;
        private Label[] _oNumsX;
        private Label[] _oNumsY;

        #endregion

        #region PublicMethod Drawing

        public void DrawDivisionsY()
        {
            if ((bool) (DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof (DependencyObject)).DefaultValue))
                return;

            Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (_dataSource.Count != 0)
                    {
                        double height = ((FrameworkElement) GraphContainer.Children[1]).ActualHeight;
                        string ytext = _axisYtext;
                        const double linesize = 8;

                        // Length of division lines.
                        double dStep = height/_ydivisions; // Distance between division lines.
                        int iLines = _ydivisions + 1; // Number of division lines.

                        ODivisionsY.Children.Clear();

                        _oLineY = new Line[iLines];
                        for (int i = 0; i < iLines; i++)
                        {
                            _oLineY[i] = new Line
                            {
                                Name = "YDivision_" + i.ToString().PadLeft(2, '0'),
                                X1 = GRAPH_BORDER - linesize,
                                Y1 = i*dStep,
                                X2 = GRAPH_BORDER,
                                Y2 = i*dStep,
                                Stroke = Brushes.Black
                            };
                            Canvas.SetRight(_oLineY[i], 0);
                            ODivisionsY.Children.Add(_oLineY[i]);
                        }

                        _oNumsY = new Label[iLines];
                        for (int i = 0; i < iLines; i++)
                        {
                            _oNumsY[i] = new Label
                            {
                                Name = "YDivisionValue_" + i.ToString().PadLeft(2, '0'),
                                Background = Brushes.Transparent,
                                BorderThickness = new Thickness(0),
                                Padding = new Thickness(0),
                                FontFamily = new FontFamily("Verdana"),
                                FontSize = 10.0,
                                HorizontalContentAlignment = HorizontalAlignment.Right,
                                VerticalContentAlignment = VerticalAlignment.Top
                            };
                            // Test: Brushes.LightBlue.

                            //dValueY = _yoriginvalue + (((_yfullscalevalue - _yoriginvalue) / _ydivisions) * i);
                            string sValueY;
                            if (dStep < 10)
                            {
                                if (i%2 == 1)
                                {
                                    int icopy = i - 1;
                                    if (icopy > -1)
                                    {
                                        sValueY = _dataSource[(icopy)].Y;
                                        //      oNumsY[i].Content = sValueY;
                                    }
                                }
                                else
                                {
                                    int icopy = i - 1;
                                    if (icopy > -1)
                                    {
                                        sValueY = _dataSource[(icopy)].Y;
                                        sValueY = sValueY.Replace("\t", "");
                                        _oNumsY[i].Content = sValueY;
                                    }
                                }
                            }
                            else
                            {
                                int icopy = i - 1;
                                if (icopy > -1)
                                {
                                    sValueY = _dataSource[(icopy)].Y;
                                    sValueY = sValueY.Replace("\t", "");
                                    _oNumsY[i].Content = sValueY;
                                }
                            }

                            _oNumsY[i].Loaded += LabelYLoaded;
                            Canvas.SetRight(_oNumsY[i], linesize + 3 + _oNumsY[i].ActualWidth);
                            Canvas.SetTop(_oNumsY[i], height - (i*dStep + 6 - (dStep/2)));
                            ODivisionsY.Children.Add(_oNumsY[i]);
                        }

                        _labelAxisY.RenderTransformOrigin = new Point(0, 0);
                        _labelAxisY.RenderTransform = new RotateTransform(270);
                        var text = new FormattedText(ytext, new CultureInfo("FR-fr"), FlowDirection.LeftToRight,
                                                     new Typeface("Verdana"), 14, Brushes.Black);
                        text.SetFontWeight(FontWeights.Bold);
                        _labelAxisY.Content = text.Text;
                        Canvas.SetTop(_labelAxisY, (height/2) + (text.Width/2));
                        Canvas.SetLeft(_labelAxisY, -10);

                        ODivisionsY.Children.Add(_labelAxisY);
                    }
                }), DispatcherPriority.Render);
        }

        public void DrawDivisionsX()
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (_dataSource.Count != 0)
                    {
                        double width = ((FrameworkElement) GraphContainer.Children[0]).ActualWidth;
                        string xtext = _axisXtext;
                        const double linesize = 8.0; // Length of division lines.
                        const double textheight = 11.0; // Height of x-axis text labels.
                        double dStep = width/(_xdivisions - 1); // Distance between division lines.
                        int iLines = _xdivisions + 1; // Number of division lines.

                        // Remove divisions, text and numbers.
                        ODivisionsX.Children.Clear();

                        // X-divisions.
                        _oLineX = new Line[iLines];
                        for (int i = 0; i < iLines - 1; i++)
                        {
                            _oLineX[i] = new Line
                            {
                                Name = "XDivision_" + i.ToString().PadLeft(2, '0'),
                                X1 = i*dStep,
                                Y1 = 0,
                                X2 = i*dStep
                            };
                            if (i%2 == 0)
                            {
                                _oLineX[i].Y2 = linesize + 7;
                                _oLineX[i].StrokeThickness = 2;
                            }
                            else
                            {
                                _oLineX[i].Y2 = linesize;
                            }
                            _oLineX[i].Stroke = Brushes.Black;
                            ODivisionsX.Children.Add(_oLineX[i]);
                        }

                        _oNumsX = new Label[iLines];
                        for (int i = 0; i < iLines; i++)
                        {
                            _oNumsX[i] = new Label
                            {
                                Name = "XDivisionValue_" + i.ToString().PadLeft(2, '0'),
                                Height = textheight,
                                Background = Brushes.Transparent,
                                BorderThickness = new Thickness(0),
                                Padding = new Thickness(0),
                                FontFamily = new FontFamily("Verdana"),
                                FontSize = 10.0,
                                HorizontalContentAlignment = HorizontalAlignment.Left,
                                VerticalContentAlignment = VerticalAlignment.Top
                            };

                            // Test: Brushes.LightBlue.

                            //dValueX = _xoriginvalue + (((_xfullscalevalue - _xoriginvalue) / _xdivisions) * i);


                            string sValueX;
                            if (dStep < 10 && dStep >= 5)
                            {
                                if (i%2 == 1)
                                {
                                    int icopy = i - 1;
                                    if (icopy > -1)
                                    {
                                        sValueX = _dataSource[(icopy*_ydivisions)].X;
                                        _oNumsX[i].Content = sValueX.Replace("\t", "");
                                        _oNumsX[i].Tag = sValueX;
                                    }

                                    #region WidthWLabel

                                    if (_oNumsX[i].Tag != null)
                                    {
                                        if (Math.Abs(WidthXLabel - (-1)) > double.Epsilon)
                                        {
                                            _oNumsX[i].Width = WidthXLabel;
                                            var text = new FormattedText(_oNumsX[i].Tag.ToString().Replace("\t", ""),
                                                                         new CultureInfo("FR-fr"),
                                                                         FlowDirection.LeftToRight,
                                                                         new Typeface("Verdana"), 14, Brushes.Black);
                                            bool wrap = false;

                                            while (text.Width > WidthXLabel)
                                            {
                                                string test = text.Text.Remove(text.Text.Length - 1);
                                                test = test.Replace("\t", "");
                                                text = new FormattedText(test, new CultureInfo("FR-fr"),
                                                                         FlowDirection.LeftToRight,
                                                                         new Typeface("Verdana"), 14, Brushes.Black);
                                                wrap = true;
                                            }
                                            string test2 = text.Text;

                                            if (wrap)
                                            {
                                                for (int w = 0; w < 3; w++)
                                                {
                                                    test2 += ".";
                                                }
                                            }

                                            var str = test2;

                                            _oNumsX[i].Content = str;
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    int icopy = i - 1;
                                    if (icopy > -1)
                                    {
                                        sValueX = _dataSource[(icopy*_ydivisions)].X;
                                        _oNumsX[i].Tag = sValueX;
                                    }
                                }
                            }
                            else if (dStep < 5)
                            {
                                if (i%5 == 1)
                                {
                                    int icopy = i - 1;
                                    if (icopy > -1)
                                    {
                                        sValueX = _dataSource[(icopy*_ydivisions)].X;
                                        _oNumsX[i].Content = sValueX.Replace("\t", "");
                                        _oNumsX[i].Tag = sValueX;
                                    }

                                    #region WidthWLabel

                                    if (_oNumsX[i].Tag != null)
                                    {
                                        if (Math.Abs(WidthXLabel - (-1)) > double.Epsilon)
                                        {
                                            string str = "";
                                            _oNumsX[i].Width = WidthXLabel;
                                            var text = new FormattedText(_oNumsX[i].Tag.ToString().Replace("\t", ""),
                                                                         new CultureInfo("FR-fr"),
                                                                         FlowDirection.LeftToRight,
                                                                         new Typeface("Verdana"), 14, Brushes.Black);
                                            str = text.Text;
                                            bool wrap = false;

                                            while (text.Width > WidthXLabel)
                                            {
                                                string test = text.Text.Remove(text.Text.Length - 1);
                                                test = test.Replace("\t", "");
                                                text = new FormattedText(test, new CultureInfo("FR-fr"),
                                                                         FlowDirection.LeftToRight,
                                                                         new Typeface("Verdana"), 14, Brushes.Black);
                                                wrap = true;
                                            }
                                            string test2 = text.Text;

                                            if (wrap)
                                            {
                                                for (int w = 0; w < 3; w++)
                                                {
                                                    test2 += ".";
                                                }
                                            }

                                            str = test2;

                                            _oNumsX[i].Content = str;
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    int icopy = i - 1;
                                    if (icopy > -1)
                                    {
                                        sValueX = _dataSource[(icopy*_ydivisions)].X;
                                        _oNumsX[i].Tag = sValueX;
                                    }
                                }
                            }
                            else
                            {
                                int icopy = i - 1;
                                if (icopy > -1)
                                {
                                    sValueX = _dataSource[(icopy*_ydivisions)].X;
                                    _oNumsX[i].Content = sValueX.Replace("\t", "");
                                    _oNumsX[i].Tag = sValueX;
                                }

                                #region WidthWLabel

                                if (_oNumsX[i].Tag != null)
                                {
                                    if (Math.Abs(WidthXLabel - (-1)) > double.Epsilon)
                                    {
                                        _oNumsX[i].Width = WidthXLabel;
                                        var text = new FormattedText(_oNumsX[i].Tag.ToString().Replace("\t", ""),
                                                                     new CultureInfo("FR-fr"), FlowDirection.LeftToRight,
                                                                     new Typeface("Verdana"), 14, Brushes.Black);
                                        bool wrap = false;

                                        while (text.Width > WidthXLabel)
                                        {
                                            string test = text.Text.Remove(text.Text.Length - 1);
                                            test = test.Replace("\t", "");
                                            text = new FormattedText(test, new CultureInfo("FR-fr"),
                                                                     FlowDirection.LeftToRight, new Typeface("Verdana"),
                                                                     14, Brushes.Black);
                                            wrap = true;
                                        }
                                        string test2 = text.Text;

                                        if (wrap)
                                        {
                                            for (int w = 0; w < 3; w++)
                                            {
                                                test2 += ".";
                                            }
                                        }

                                        var str = test2;

                                        _oNumsX[i].Content = str;
                                    }
                                }

                                #endregion
                            }
                            _oNumsX[i].Margin = new Thickness(0, 0, 0, 0);
                            _oNumsX[i].RenderTransformOrigin = new Point(0, 0);
                            _oNumsX[i].RenderTransform = new RotateTransform(90);

                            Canvas.SetTop(_oNumsX[i], 20);
                            int icopy2 = i - 1;
                            if (i < 0)
                            {
                                icopy2 = 0;
                            }
                            _oNumsX[i].Loaded += LabelXLoaded;
                            Canvas.SetLeft(_oNumsX[i], (icopy2)*dStep + 7);
                            ODivisionsX.Children.Add(_oNumsX[i]);
                        }


                        _labelAxisX.Content = xtext;
                        Canvas.SetLeft(_labelAxisX, width/2);

                        ODivisionsX.Children.Add(_labelAxisX);
                    }
                }), DispatcherPriority.Render);
            Canvas.SetTop(_labelAxisX, MaxWidthXLabel.Value - 10);
        }

        public void DrawChartArea()
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    List<ThermoDataPoint> tmpList = _dataSource;


                    GraphArea.Children.Clear();
                    GraphArea.ColumnDefinitions.Clear();
                    GraphArea.RowDefinitions.Clear();
                    //GraphArea.SnapsToDevicePixels = true;

                    for (int i = 0; i < _xdivisions - 1; i++)
                    {
                        var coldef = new ColumnDefinition();
                        GraphArea.ColumnDefinitions.Add(coldef);
                    }

                    for (int i = 0; i < _ydivisions; i++)
                    {
                        var rowDef = new RowDefinition();
                        GraphArea.RowDefinitions.Add(rowDef);
                    }

                    for (int k = 0; k < _xdivisions - 1; k++)
                    {
                        for (int l = 0; l < _ydivisions; l++)
                        {
                            var grd = new ThermoChartColorBlock
                            {
                                XPos = k,
                                YPos = l
                            };

                            for (int i = 0; i < _leveldef; i++)
                            {
                                var coldef = new ColumnDefinition();
                                grd.ColumnDefinitions.Add(coldef);
                            }

                            for (int i = 0; i < _leveldef; i++)
                            {
                                var rowDef = new RowDefinition();
                                grd.RowDefinitions.Add(rowDef);
                            }

                            Grid.SetColumn(grd, k);
                            Grid.SetRow(grd, l);

                            if (_oNumsX != null)
                            {
                                List<ThermoDataPoint> s;

                                s = GetRangeByWaypoint(tmpList, _oNumsX[k
                                                                        + 2].Tag.ToString());

                                if (l != 0)
                                {
                                    grd.YVal = s[s.Count - l - 1].Y;
                                    grd.ZVal = s[s.Count - l - 1].Z;
                                }
                                else
                                {
                                    grd.YVal = s[s.Count - 1].Y;
                                    grd.ZVal = s[s.Count - 1].Z;
                                }

                                string a = "";
                                string b = "";

                                b = tmpList[(k*_ydivisions)].X;
                                try
                                {
                                    a = tmpList[((k + 1)*_ydivisions)].X;
                                }
                                catch (Exception)
                                {
                                    a = "lol";
                                }
                                grd.XVal = b + " à " + a;
                            }
                            KeyValuePair<double, ThermoChartLevel>? first;
                            KeyValuePair<double, ThermoChartLevel>? last;

                            GetLevelByVal(grd.ZVal, out first, out last);
                            grd.Background = new SolidColorBrush(GetColorBetweenLevel(grd.ZVal, first, last));

                            _thermoChartColorBlockList.Add(grd);
                            GraphArea.Children.Add(grd);

                            if (_leveldef != 1)
                            {
                                for (int i = 0; i < _leveldef; i++)
                                {
                                    for (int j = 0; j < _leveldef; j++)
                                    {
                                        var t = new Grid();
                                        int red = _r.Next(0, byte.MaxValue + 1);
                                        int green = _r.Next(0, byte.MaxValue + 1);
                                        int blue = _r.Next(0, byte.MaxValue + 1);

                                        Brush brush =
                                            new SolidColorBrush(Color.FromRgb((byte) red, (byte) green, (byte) blue));
                                        t.Background = brush;
                                        Grid.SetColumn(t, i);
                                        Grid.SetRow(t, j);
                                        grd.Children.Add(t);
                                    }
                                }
                            }
                        }
                    }
                }), DispatcherPriority.Render);
        }

        #endregion

        #region PrivateMethod Utility

        private List<ThermoDataPoint> GetRangeByWaypoint(List<ThermoDataPoint> lst, string arr)
        {
            var lst2 = new List<ThermoDataPoint>();
            lst2 = lst.Where(arret => arret.X == arr).ToList();
            return lst2;
        }

        private UIElement GetChildAt(Grid grid, int row, int column)
        {
            foreach (UIElement element in grid.Children)
            {
                if (Grid.GetColumn(element) == column && Grid.GetRow(element) == row)
                {
                    return element;
                }
            }
            throw new Exception("Could not find UIElement");
        }

        private List<ThermoChartColorBlock> GetThermoChartColorBlockBetweenTwoPoints(Point startPoint, Point endPoint)
        {
            var lst = new List<ThermoChartColorBlock>();

            foreach (ThermoChartColorBlock item in _thermoChartColorBlockList)
            {
                #region IfCase1

                if (item.YPos >= startPoint.Y && item.YPos <= endPoint.Y && item.XPos >= endPoint.X &&
                    item.XPos <= startPoint.X)
                {
                    if (item.YPos == startPoint.Y && item.YPos == endPoint.Y && item.XPos == startPoint.X &&
                        item.XPos == endPoint.X)
                    {
                        item.IsOnlyOne = true;
                    }

                    else if (item.YPos == startPoint.Y && item.YPos == endPoint.Y && item.XPos == startPoint.X)
                    {
                        item.IsBottomTopLeftSelected = true;
                    }

                    else if (item.XPos == startPoint.X && item.XPos == endPoint.X && item.YPos == startPoint.Y)
                    {
                        item.IsLeftRightTopSelected = true;
                    }

                    else if (item.YPos == startPoint.Y && item.YPos == endPoint.Y && item.XPos == endPoint.X)
                    {
                        item.IsBottomTopRightSelected = true;
                    }

                    else if (item.YPos == endPoint.Y && item.XPos == startPoint.X && item.XPos == endPoint.X)
                    {
                        item.IsLeftRightBottomSelected = true;
                    }
                    else if (item.YPos == startPoint.Y && item.XPos == startPoint.X)
                    {
                        item.IsTopRightSelected = true;
                    }
                    else if (item.YPos == endPoint.Y && item.XPos == endPoint.X)
                    {
                        item.IsBottomLeftSelected = true;
                    }
                    else if (item.YPos == startPoint.Y && item.XPos == endPoint.X)
                    {
                        item.IsTopLeftSelected = true;
                    }
                    else if (item.YPos == endPoint.Y && item.XPos == startPoint.X)
                    {
                        item.IsBottomRightSelected = true;
                    }
                    else if (item.YPos == startPoint.Y && item.YPos == endPoint.Y)
                    {
                        item.IsBottomTopSelected = true;
                    }
                    else if (item.XPos == startPoint.X && item.XPos == endPoint.X)
                    {
                        item.IsLeftRightSelected = true;
                    }
                    else if (item.YPos == startPoint.Y)
                    {
                        item.IsTopSelected = true;
                    }
                    else if (item.YPos == endPoint.Y)
                    {
                        item.IsBottomSelected = true;
                    }
                    else if (item.XPos == startPoint.X)
                    {
                        item.IsRightSelected = true;
                    }
                    else if (item.XPos == endPoint.X)
                    {
                        item.IsLeftSelected = true;
                    }
                    else
                    {
                        item.IsCenter = true;
                    }
                } 
                    #endregion
                    #region IfCase2

                else if (item.YPos >= endPoint.Y && item.YPos <= startPoint.Y && item.XPos >= startPoint.X &&
                         item.XPos <= endPoint.X)
                {
                    if (item.YPos == startPoint.Y && item.YPos == endPoint.Y && item.XPos == startPoint.X &&
                        item.XPos == endPoint.X)
                    {
                        item.IsOnlyOne = true;
                    }

                    else if (item.YPos == startPoint.Y && item.YPos == endPoint.Y && item.XPos == startPoint.X)
                    {
                        item.IsBottomTopLeftSelected = true;
                    }

                    else if (item.XPos == startPoint.X && item.XPos == endPoint.X && item.YPos == startPoint.Y)
                    {
                        item.IsLeftRightTopSelected = true;
                    }

                    else if (item.YPos == startPoint.Y && item.YPos == endPoint.Y && item.XPos == endPoint.X)
                    {
                        item.IsBottomTopRightSelected = true;
                    }

                    else if (item.YPos == endPoint.Y && item.XPos == startPoint.X && item.XPos == endPoint.X)
                    {
                        item.IsLeftRightBottomSelected = true;
                    }
                    else if (item.YPos == startPoint.Y && item.XPos == startPoint.X)
                    {
                        item.IsBottomLeftSelected = true;
                    }
                    else if (item.YPos == endPoint.Y && item.XPos == endPoint.X)
                    {
                        item.IsTopRightSelected = true;
                    }
                    else if (item.YPos == startPoint.Y && item.XPos == endPoint.X)
                    {
                        item.IsBottomRightSelected = true;
                    }
                    else if (item.YPos == endPoint.Y && item.XPos == startPoint.X)
                    {
                        item.IsTopLeftSelected = true;
                    }
                    else if (item.YPos == startPoint.Y && item.YPos == endPoint.Y)
                    {
                        item.IsBottomTopSelected = true;
                    }
                    else if (item.XPos == startPoint.X && item.XPos == endPoint.X)
                    {
                        item.IsLeftRightSelected = true;
                    }
                    else if (item.YPos == startPoint.Y)
                    {
                        item.IsBottomSelected = true;
                    }
                    else if (item.YPos == endPoint.Y)
                    {
                        item.IsTopSelected = true;
                    }
                    else if (item.XPos == startPoint.X)
                    {
                        item.IsLeftSelected = true;
                    }
                    else if (item.XPos == endPoint.X)
                    {
                        item.IsRightSelected = true;
                    }
                    else
                    {
                        item.IsCenter = true;
                    }
                } 
                    #endregion
                    #region IfCase3

                else
                {
                    if (item.YPos == startPoint.Y && item.YPos == endPoint.Y && item.XPos == startPoint.X &&
                        item.XPos == endPoint.X)
                    {
                        item.IsOnlyOne = true;
                    }

                    else if (item.YPos == startPoint.Y && item.YPos == endPoint.Y && item.XPos == startPoint.X)
                    {
                        item.IsBottomTopLeftSelected = true;
                    }

                    else if (item.XPos == startPoint.X && item.XPos == endPoint.X && item.YPos == startPoint.Y)
                    {
                        item.IsLeftRightTopSelected = true;
                    }

                    else if (item.YPos == startPoint.Y && item.YPos == endPoint.Y && item.XPos == endPoint.X)
                    {
                        item.IsBottomTopRightSelected = true;
                    }

                    else if (item.YPos == endPoint.Y && item.XPos == startPoint.X && item.XPos == endPoint.X)
                    {
                        item.IsLeftRightBottomSelected = true;
                    }
                    else if (item.YPos == startPoint.Y && item.XPos == startPoint.X)
                    {
                        item.IsTopLeftSelected = true;
                    }
                    else if (item.YPos == endPoint.Y && item.XPos == endPoint.X)
                    {
                        item.IsBottomRightSelected = true;
                    }
                    else if (item.YPos == startPoint.Y && item.XPos == endPoint.X)
                    {
                        item.IsTopRightSelected = true;
                    }
                    else if (item.YPos == endPoint.Y && item.XPos == startPoint.X)
                    {
                        item.IsBottomLeftSelected = true;
                    }
                    else if (item.YPos == startPoint.Y && item.YPos == endPoint.Y)
                    {
                        item.IsBottomTopSelected = true;
                    }
                    else if (item.XPos == startPoint.X && item.XPos == endPoint.X)
                    {
                        item.IsLeftRightSelected = true;
                    }
                    else if (item.YPos == startPoint.Y)
                    {
                        item.IsTopSelected = true;
                    }
                    else if (item.YPos == endPoint.Y)
                    {
                        item.IsBottomSelected = true;
                    }
                    else if (item.XPos == startPoint.X)
                    {
                        item.IsLeftSelected = true;
                    }
                    else if (item.XPos == endPoint.X)
                    {
                        item.IsRightSelected = true;
                    }
                    else
                    {
                        item.IsCenter = true;
                    }
                }

                #endregion

                if (item.YPos >= startPoint.Y && item.YPos <= endPoint.Y && item.XPos >= startPoint.X &&
                    item.XPos <= endPoint.X)
                {
                    if (!string.IsNullOrEmpty(item.XVal))
                    {
                        lst.Add(item);
                    }
                    item.IsHover = true;
                }
                else if (item.YPos >= startPoint.Y && item.YPos <= endPoint.Y && item.XPos >= endPoint.X &&
                         item.XPos <= startPoint.X)
                {
                    if (!string.IsNullOrEmpty(item.XVal))
                    {
                        lst.Add(item);
                    }
                    item.IsHover = true;
                }
                else if (item.YPos >= endPoint.Y && item.YPos <= startPoint.Y && item.XPos >= startPoint.X &&
                         item.XPos <= endPoint.X)
                {
                    if (!string.IsNullOrEmpty(item.XVal))
                    {
                        lst.Add(item);
                    }
                    item.IsHover = true;
                }
            }

            return lst;
        }

        #endregion

        #region PrivateMethod Value->Color

        private void GetLevelByVal(double value, out KeyValuePair<double, ThermoChartLevel>? first,
                                   out KeyValuePair<double, ThermoChartLevel>? last)
        {
            ThermoChart curChart = ThermoChartUtility.GetCurrentChart(this);
            var sup = new Dictionary<double, ThermoChartLevel>();
            var inf = new Dictionary<double, ThermoChartLevel>();

            sup = curChart.LevelsDictionnary.Where(kvp => kvp.Key >= value).ToDictionary(kvp => kvp.Key,
                                                                                          kvp => kvp.Value);
            inf = curChart.LevelsDictionnary.Where(kvp => kvp.Key <= value).ToDictionary(kvp => kvp.Key,
                                                                                          kvp => kvp.Value);

            if (inf.Count != 0)
            {
                first = inf.ElementAt(inf.Count - 1);
            }
            else
            {
                first = null;
            }

            if (sup.Count != 0)
            {
                last = sup.ElementAt(0);
            }
            else
            {
                last = null;
            }
        }

        private Color GetColorBetweenLevel(double val, KeyValuePair<double, ThermoChartLevel>? first,
                                           KeyValuePair<double, ThermoChartLevel>? last)
        {
            if (first != null && last != null)
            {
                IEnumerable<Color> colors = GetGradients(first.Value.Value.StartColor, first.Value.Value.EndColor, 20);
                List<Color> testcolor = colors.ToList();
                double ecart;
                if (val - first.Value.Key == 0)
                {
                    return testcolor[0];
                    //return testcolor[testcolor.Count - 1];
                }
                if (first.Value.Key != 0)
                {
                    val = val - first.Value.Key;
                    ecart = ((val*20)/(last.Value.Key - first.Value.Key));
                }
                else
                {
                    ecart = (val*20)/last.Value.Key;
                }
                ecart = Math.Round(ecart, 0);
                var test = (int) ecart;
                if (test > 19)
                {
                    test = 19;
                }
                else if (test < 0)
                {
                    test = 0;
                }
                Color clr = testcolor[test];
                return clr;
            }
            else
            {
                Color clr;
                //if (first != null)
                if (val != 0 && first != null)
                {
                    clr = first.Value.Value.EndColor;
                }
                else if (last != null)
                {
                    // return Colors.White;
                    clr = last.Value.Value.StartColor;
                }
                else
                {
                    clr = Colors.White;
                }
                return clr;
                //return Colors.White;
            }
        }

        public IEnumerable<Color> GetGradients(Color start, Color end, int steps)
        {
            Color stepper = Color.FromArgb((byte) ((end.A - start.A)/(steps - 1)),
                                           (byte) ((end.R - start.R)/(steps - 1)),
                                           (byte) ((end.G - start.G)/(steps - 1)),
                                           (byte) ((end.B - start.B)/(steps - 1)));

            for (int i = 0; i < steps; i++)
            {
                yield return Color.FromArgb((byte) (start.A + (stepper.A*i)),
                                            (byte) (start.R + (stepper.R*i)),
                                            (byte) (start.G + (stepper.G*i)),
                                            (byte) (start.B + (stepper.B*i)));
            }
        }

        #endregion

        #region PrivateMethod AutoSize

        private void LabelYLoaded(object sender, RoutedEventArgs e)
        {
            var lbl = sender as Label;
            if (lbl != null && lbl.ActualWidth > _maxWidthYLabel.Value)
            {
                MaxWidthYLabel = new GridLength(lbl.ActualWidth + 35);
            }
        }

        private void LabelXLoaded(object sender, RoutedEventArgs e)
        {
            if (_widthXLabel != -1) return;
            var lbl = sender as Label;
            if (lbl != null && lbl.ActualWidth > _maxWidthXLabel.Value)
            {
                MaxWidthXLabel = new GridLength(lbl.ActualWidth + 10);
            }
        }

        private void ClearAxisX()
        {
            if (_oNumsX != null)
            {
                foreach (Label lb in _oNumsX)
                {
                    lb.Loaded -= LabelXLoaded;
                }
            }
        }

        private void ClearAxisY()
        {
            if (_oNumsY != null)
            {
                foreach (Label lb in _oNumsY)
                {
                    lb.Loaded -= LabelYLoaded;
                }
            }
        }

        private void ClearDataArea()
        {
            if (_thermoChartColorBlockList.Count > 0)
            {
                foreach (ThermoChartColorBlock item in _thermoChartColorBlockList)
                {
                    item.Brd.MouseDown -= item.brd_MouseDown;
                    item.Brd.MouseMove -= item.brd_MouseMove;
                }
                _thermoChartColorBlockList.Clear();
            }
        }

        #endregion

        #region PublicMethodUtility

        public void GetAvgMaxMinColorForSelectedBlocks(out double avg, out double max, out double min,
                                                       out Brush[] colors)
        {
            colors = new Brush[3];
            avg = _thermoChartColorBlockSelectedList.Average(sel => sel.ZVal);
            max = _thermoChartColorBlockSelectedList.Max(sel => sel.ZVal);
            min = _thermoChartColorBlockSelectedList.Min(sel => sel.ZVal);

            KeyValuePair<double, ThermoChartLevel>? first;
            KeyValuePair<double, ThermoChartLevel>? last;

            GetLevelByVal(avg, out first, out last);
            colors[0] = new SolidColorBrush(GetColorBetweenLevel(avg, first, last));

            GetLevelByVal(max, out first, out last);
            colors[1] = new SolidColorBrush(GetColorBetweenLevel(max, first, last));

            GetLevelByVal(min, out first, out last);
            colors[2] = new SolidColorBrush(GetColorBetweenLevel(min, first, last));
        }

        public void ResetClip()
        {
            GraphArea.Clip = new RectangleGeometry(new Rect(0, 0, GraphArea.ActualWidth, GraphArea.ActualHeight));
        }

        #endregion
    }
}