//============================================================
// COPYRIGHT: TDE TRANSDATA France
//============================================================
// $Workfile: Histo_Data_Columns.cs $
//
// PROJECT : Stathor
// VERSION : 1.0
// CREATION : 25-08-2011
// AUTHOR : Mickael Hebras
//
// DETAILS : Objet qui permet de stocker les infos pour une bar de l'histogramme
//
//-- REVISION ------------------------------------------------
//
// $Revision: $
// $Date: $
// $Author: $
//
// $NoKeywords: $
//
//------------------------------------------------------------

namespace ThermoChart_Control
{
    public class HistoDataColumns
    {
        #region Property

        public HistoDataColumns(string x, double min, double moy, double max, int distance)
        {
            X = x;
            Min = min;
            Moy = moy;
            Max = max;
            Distance = distance;
        }

        public double Min { get; set; }

        public double Max { get; set; }

        public double Moy { get; set; }

        public string X { get; set; }

        public double Distance { get; set; }

        #endregion
    }
}