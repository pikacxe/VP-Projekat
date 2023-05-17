using System;
using System.Collections.Concurrent;

namespace InMemoryDB
{
    public class DataBase
    {
        static ConcurrentDictionary<int, Load> loads = new ConcurrentDictionary<int, Load>();
        static ConcurrentDictionary<int, ImportedFile> importedfiles= new ConcurrentDictionary<int, ImportedFile>();
        static ConcurrentDictionary<int, Audit> audits=new ConcurrentDictionary<int, Audit>();

        public void AddLoad(Load load)
        {
            if (Contains(load.TimeStamp))
            {
                return;
            }
            load.ID = loads.Count;
            loads.TryAdd(load.ID, load);
        }

        public void AddImportedFile(ImportedFile importedFile)
        {
            importedFile.ID = importedfiles.Count;
            importedfiles.TryAdd(importedFile.ID, importedFile);
        }

        public void AddAudit(Audit audit)
        {
            audit.ID = audits.Count;
            audits.TryAdd(audit.ID, audit);
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

        public static bool Contains(DateTime timeStamp)
        {
            foreach (var x in loads.Values)
            {
                if (timeStamp == x.TimeStamp)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
