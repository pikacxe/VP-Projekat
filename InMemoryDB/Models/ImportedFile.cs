﻿namespace InMemoryDB
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

        public ImportedFile(string fileName)
        {
            _FileName = fileName;
        }

        public override string ToString()
        {
            return $"{ID}\t{FileName}";
        }

        public static string FormatHeader()
        {
            return "\n==========IMPORTED FILES==========\nID\tFILENAME";
        }
    }
}
