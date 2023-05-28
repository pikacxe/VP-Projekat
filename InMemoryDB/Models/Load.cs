using System;
using System.Globalization;

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

        public float ForecastValue
        {
            get { return _ForecastValue; }
            set { _ForecastValue = value; }
        }

        public float MeasuredValue
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

        public Load() : this(new DateTime(), -1,-1)
        {

        }
        // Returns string representation for csv header
        public static string CsvHeader()
        {
            return "DATE,TIME,FORECAST_VALUE,MEASURED_VALUE\n";
        }
        // Returns csv formated string data
        public string ToCsv()
        {
            return $"{_TimeStamp.Date.ToString("yyyy-MM-dd")},{_TimeStamp.ToString("HH:mm")},{_ForecastValue.ToString("0.00",CultureInfo.InvariantCulture)},{_MeasuredValue.ToString("0.00", CultureInfo.InvariantCulture)}\n";
        }

        public override string ToString()
        {
            return $"{ID, 3}\t{TimeStamp.ToString("yyyy.MM.dd HH:mm"), 16}\t{ForecastValue.ToString("0.00", CultureInfo.InvariantCulture), 0}\t{MeasuredValue.ToString("0.00", CultureInfo.InvariantCulture), 15}";
        }

        public static string FormatHeader()
        {
            return $"\n==========LOADS==========\n{"ID", 3}\t{"TIMESTAMP", 4}\t\t{"FORECASTVALUE", 7}\t{"MEASUREDVALUE", 7}";
        }

    }
}
