using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ThermoChart_Control
{
    internal class ThermoChartColorBlock : Grid, INotifyPropertyChanged
    {
        #region PropertyChangedEvent

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Property

        private bool _isBottomLeftSelected;
        private bool _isBottomRightSelected;
        private bool _isBottomSelected;
        private bool _isBottomTopLeftSelected;
        private bool _isBottomTopRightSelected;
        private bool _isBottomTopSelected;
        private bool _isCenter;
        private bool _isHover;
        private bool _isLeftRightBottomSelected;
        private bool _isLeftRightSelected;
        private bool _isLeftRightTopSelected;
        private bool _isLeftSelected;
        private bool _isOnlyOne;
        private bool _isRightSelected;
        private bool _isTopLeftSelected;
        private bool _isTopRightSelected;
        private bool _isTopSelected;
        public double XPos { get; set; }

        public string XVal { get; set; }

        public double YPos { get; set; }

        public string YVal { get; set; }

        public double ZVal { get; set; }


        public bool IsHover
        {
            get { return _isHover; }
            set
            {
                _isHover = value;
                if (value)
                {
                    Opacity = 0.8;
                    Brd.Visibility = Visibility.Visible;
                }
                else
                {
                    Opacity = 1;
                    Brd.Visibility = Visibility.Collapsed;
                }
            }
        }

        public bool IsSelected { get; set; }

        public bool IsTopRightSelected
        {
            get { return _isTopRightSelected; }
            set
            {
                _isTopRightSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(0, 1, 1, 0);
                    Brd.CornerRadius = new CornerRadius(0, 3, 0, 0);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsTopLeftSelected
        {
            get { return _isTopLeftSelected; }
            set
            {
                _isTopLeftSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(1, 1, 0, 0);
                    Brd.CornerRadius = new CornerRadius(3, 0, 0, 0);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsBottomLeftSelected
        {
            get { return _isBottomLeftSelected; }
            set
            {
                _isBottomLeftSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(1, 0, 0, 1);
                    Brd.CornerRadius = new CornerRadius(0, 0, 0, 3);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsBottomRightSelected
        {
            get { return _isBottomRightSelected; }
            set
            {
                _isBottomRightSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(0, 0, 1, 1);
                    Brd.CornerRadius = new CornerRadius(0, 0, 3, 0);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsBottomSelected
        {
            get { return _isBottomSelected; }
            set
            {
                _isBottomSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(0, 0, 0, 1);
                    Brd.CornerRadius = new CornerRadius(0);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsCenter
        {
            get { return _isCenter; }
            set
            {
                _isCenter = value;

                if (value)
                {
                    Brd.BorderThickness = new Thickness(0);
                    Brd.CornerRadius = new CornerRadius(0);
                }
            }
        }

        public bool IsTopSelected
        {
            get { return _isTopSelected; }
            set
            {
                _isTopSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(0, 1, 0, 0);
                    Brd.CornerRadius = new CornerRadius(0);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsRightSelected
        {
            get { return _isRightSelected; }
            set
            {
                _isRightSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(0, 0, 1, 0);
                    Brd.CornerRadius = new CornerRadius(0);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsLeftSelected
        {
            get { return _isLeftSelected; }
            set
            {
                _isLeftSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(1, 0, 0, 0);
                    Brd.CornerRadius = new CornerRadius(0);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsLeftRightSelected
        {
            get { return _isLeftRightSelected; }
            set
            {
                _isLeftRightSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(1, 0, 1, 0);
                    Brd.CornerRadius = new CornerRadius(0);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsBottomTopSelected
        {
            get { return _isBottomTopSelected; }
            set
            {
                _isBottomTopSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(0, 1, 0, 1);
                    Brd.CornerRadius = new CornerRadius(0);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsOnlyOne
        {
            get { return _isOnlyOne; }
            set
            {
                _isOnlyOne = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(1, 1, 1, 1);
                    Brd.CornerRadius = new CornerRadius(3);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsLeftRightTopSelected
        {
            get { return _isLeftRightTopSelected; }
            set
            {
                _isLeftRightTopSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(1, 1, 1, 0);
                    Brd.CornerRadius = new CornerRadius(3, 3, 0, 0);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsLeftRightBottomSelected
        {
            get { return _isLeftRightBottomSelected; }
            set
            {
                _isLeftRightBottomSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(1, 0, 1, 1);
                    Brd.CornerRadius = new CornerRadius(0, 0, 3, 3);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsBottomTopRightSelected
        {
            get { return _isBottomTopRightSelected; }
            set
            {
                _isBottomTopRightSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(0, 1, 1, 1);
                    Brd.CornerRadius = new CornerRadius(0, 3, 3, 0);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        public bool IsBottomTopLeftSelected
        {
            get { return _isBottomTopLeftSelected; }
            set
            {
                _isBottomTopLeftSelected = value;
                if (value)
                {
                    Brd.BorderThickness = new Thickness(1, 1, 0, 1);
                    Brd.CornerRadius = new CornerRadius(3, 0, 0, 3);
                }
                else
                {
                    Brd.CornerRadius = new CornerRadius(0);
                    Brd.BorderThickness = new Thickness(0);
                }
            }
        }

        #endregion
        public Border Brd { get; set; }

        #region Constructor

        public ThermoChartColorBlock()
        {
            Brd = new Border
                {
                    BorderThickness = new Thickness(0),
                    CornerRadius = new CornerRadius(0),
                    Visibility = Visibility.Collapsed,
                    BorderBrush = Brushes.Black
                };
            Brd.MouseMove += brd_MouseMove;
            Brd.MouseDown += brd_MouseDown;
            Children.Add(Brd);
            //Margin = new Thickness(0, -0.45, -0.40, -0.45);
        }

        #endregion

        #region Event

        public void brd_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }

        public void brd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        #endregion
    }
}