using InMemoryDB;
using System;
using System.Collections.Generic;

namespace Service
{
    public class GroupedLoads
    {
        private DateTime date;
        public List<Load> loads;
        public DateTime Date { get { return date; } set { date = value; } }

        public GroupedLoads(DateTime date)
        {
            this.date = date;
            loads=new List<Load>();
        }

    }
}
