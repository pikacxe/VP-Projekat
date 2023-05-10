using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryDB
{
    public class DataBase
    {
        ConcurrentDictionary<int, Load> loads = new ConcurrentDictionary<int, Load>();
        ConcurrentDictionary<int, ImportedFile> importedfiles= new ConcurrentDictionary<int, ImportedFile>();
        ConcurrentDictionary<int, Audit> audits=new ConcurrentDictionary<int, Audit>();

        public void AddLoad(Load load)
        {
            loads.TryAdd(loads.Count, load);
        }

        public void AddImportedFile(ImportedFile importedFile)
        {
            importedfiles.TryAdd(importedfiles.Count, importedFile);
        }

        public void AddAudit(Audit audit)
        {
            audits.TryAdd(audits.Count, audit);
        }

        private DataBase() { }

        private static readonly Lazy<DataBase> lazyInstance = new Lazy<DataBase>(() =>
        {
            return new DataBase();
        });

        public static DataBase Instance
        {
            get
            {
                return lazyInstance.Value;
            }
        }


    }
}
