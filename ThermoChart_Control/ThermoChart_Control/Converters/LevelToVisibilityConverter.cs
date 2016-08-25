// <copyright file="LevelToVisibilityConverter.cs" company="$registerdorganization$">
// Copyright (c) 2011 Microsoft. All Right Reserved
// </copyright>
// <author>Quentin</author>
// <email></email>
// <date>2011-07-23</date>
// <summary>A value converter for WPF and Silverlight data binding</summary>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ThermoChart_Control.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class LevelToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI. 
        /// </summary>
        /// <param name="value">The source data being passed to the target </param>
        /// <param name="targetType">The Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property. </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isLast = (bool) value;
            if (isLast)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings. 
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}