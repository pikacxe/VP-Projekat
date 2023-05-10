using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryDB
{
    public class Load
    {
        private int _ID;
        private DateTime _Timestamp;
        private float _ForecastValue;
        private float _MeasuredValue;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public DateTime Timestamp
        {
            get { return _Timestamp; }
            set { _Timestamp = value; }
        }

        private float ForecastValue
        {
            get { return _ForecastValue; }
            set { _ForecastValue = value; }
        }

        private float MeasuredValue
        {
            get { return MeasuredValue; }
            set { MeasuredValue = value; }
        }

        public Load(int iD, DateTime timestamp, float forecastValue, float measuredValue)
        {
            _ID = iD;
            _Timestamp = timestamp;
            _ForecastValue = forecastValue;
            _MeasuredValue = measuredValue;
        }
    }
}
