namespace ThermoChart_Control
{
    public class ThermoChartStatus
    {
        #region Property

        private ThermoChartEnumStatus _datasStatus;
        private ThermoChartEnumStatus _levelStatus;

        internal ThermoChartEnumStatus LevelStatus
        {
            get { return _levelStatus; }
            set { _levelStatus = value; }
        }

        internal ThermoChartEnumStatus DatasStatus
        {
            get { return _datasStatus; }
            set { _datasStatus = value; }
        }

        #endregion

        #region Constructor

        public ThermoChartStatus()
        {
            _datasStatus = ThermoChartEnumStatus.Unknown;
            _levelStatus = ThermoChartEnumStatus.Unknown;
        }

        #endregion

        #region PublicMethod

        public string GetMessage()
        {
            string str = "";
            switch (_datasStatus)
            {
                case ThermoChartEnumStatus.Valid:
                    str += "Données Valides /";
                    break;
                case ThermoChartEnumStatus.Invalid:
                    str += "Données Invalides /";
                    break;
                case ThermoChartEnumStatus.Unknown:
                    str += "Etat des données inconnu /";
                    break;
            }
            switch (_levelStatus)
            {
                case ThermoChartEnumStatus.Valid:
                    str += "Paliers Valides";
                    break;
                case ThermoChartEnumStatus.Invalid:
                    str += "Paliers Invalides";
                    break;
                case ThermoChartEnumStatus.Unknown:
                    str += "Etat des paliers inconnu";
                    break;
            }

            return str;
        }

        public override string ToString()
        {
            return GetMessage();
        }

        #endregion
    }
}