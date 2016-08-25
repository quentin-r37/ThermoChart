namespace ThermoChart_Control
{
    public class ThermoDataPoint
    {
        #region Property

        private readonly string _x;

        private readonly string _y;

        private readonly double _z;

        public string X => _x;

        public string Y => _y;

        public double Z => _z;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a Thermo_Data_Point
        /// </summary>
        /// <param name="x">Value X</param>
        /// <param name="y">Value Y</param>
        /// <param name="z">Value Z</param>
        public ThermoDataPoint(string x, string y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        #endregion

        #region PublicMethod

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (ThermoDataPoint)) return false;

            if (((ThermoDataPoint) obj)._x == _x &&
                ((ThermoDataPoint) obj)._y == _y &&
                ((ThermoDataPoint) obj)._z == _z)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return new {z = _z, x = _x, y = _y}.GetHashCode();
        }

        public override string ToString()
        {
            return X + "/" + Y + "/" + Z;
        }

        #endregion
    }
}