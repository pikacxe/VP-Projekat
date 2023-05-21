using Common.CustomEvents;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace InMemoryDB
{
    public class DataBase
    {
        // Create new concurrent dictionaries that represent database
        static ConcurrentDictionary<int, Load> loads = new ConcurrentDictionary<int, Load>();
        static ConcurrentDictionary<int, ImportedFile> importedfiles = new ConcurrentDictionary<int, ImportedFile>();
        static ConcurrentDictionary<int, Audit> audits = new ConcurrentDictionary<int, Audit>();

        // Getters for respective dictionaries
        public IEnumerable<Load> Loads { get { return loads.Values; } }
        public IEnumerable<Audit> Audits { get { return audits.Values; } }
        public IEnumerable<ImportedFile> ImportedFiles { get { return importedfiles.Values; } }     
        public bool AddLoad(Load load)
        {
            // Checks if load for provided timeStamp is already in database
            if (Contains(load.TimeStamp))
            {
                return false;
            }
            load.ID = loads.Count;
            return loads.TryAdd(load.ID, load);
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

        // Check if dictionary loads already contains object load with same timeStamp value
        public static bool Contains(DateTime timeStamp)
        {
            foreach (var x in loads.Values)
            {
                if (timeStamp == x.TimeStamp)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
