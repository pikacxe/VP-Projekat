using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryDB
{
    public class ImportedFile
    {
        private int _ID;
        private string _FileName;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        public ImportedFile(int iD, string fileName)
        {
            _ID = iD;
            _FileName = fileName;
        }
    }
}
