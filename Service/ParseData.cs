using InMemoryDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ParseData
    {
        private DateTime date;
        public List<Load> loads;
        public DateTime Date { get { return date; } set { date = value; } }

        public ParseData(DateTime date)
        {
            this.date = date;
            loads=new List<Load>();
        }

    }
}
