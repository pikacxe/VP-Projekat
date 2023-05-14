using System;

namespace InMemoryDB
{
    public class Load
    {
        private int _ID;
        private DateTime _TimeStamp;
        private float _ForecastValue;
        private float _MeasuredValue;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public DateTime TimeStamp
        {
            get { return _TimeStamp; }
            set { _TimeStamp = value; }
        }

        private float ForecastValue
        {
            get { return _ForecastValue; }
            set { _ForecastValue = value; }
        }

        private float MeasuredValue
        {
            get { return _MeasuredValue; }
            set { _MeasuredValue = value; }
        }

        public Load( DateTime timestamp, float forecastValue, float measuredValue)
        {
            _TimeStamp = timestamp;
            _ForecastValue = forecastValue;
            _MeasuredValue = measuredValue;
        }

        public static string CsvHeader()
        {
            return "TIME_STAMP,FORECAST_VALUE,MEASURED_VALUE\n";
        }
        public string ToCsv()
        {
            return $"{_TimeStamp.Date.ToString("yyyy-MM-dd")},{_TimeStamp.ToString("HH:mm")},{_ForecastValue},{_MeasuredValue}\n";
        }
    }
}
