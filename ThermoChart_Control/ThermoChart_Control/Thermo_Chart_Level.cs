using System.Windows.Media;

namespace ThermoChart_Control
{
    public class ThermoChartLevel
    {
        #region Property

        public Color StartColor { get; set; }

        public Color EndColor { get; set; }

        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        #endregion

        /// <summary>
        /// Créer une instance de thermo_chart_level
        /// </summary>
        /// <param name="startcolor">Couleur de départ</param>
        /// <param name="endcolor">Couleur d'arrivée</param>
        /// <param name="minvalue">Valeur Minimum</param>
        /// <param name="maxvalue">Valeur Maximum</param>
        public ThermoChartLevel(Color startcolor, Color endcolor, double minvalue, double maxvalue)
        {
            StartColor = startcolor;
            EndColor = endcolor;
            MinValue = minvalue;
            MaxValue = maxvalue;
        }
    }
}