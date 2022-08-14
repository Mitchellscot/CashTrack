using CashTrack.Data.Entities;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CashTrack.Data.CsvFiles
{
    public class CsvUtility<T> where T : class
    {
        public IEnumerable<T> GetEntitiesFromCSV(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records =  csv.GetRecords<T>();
                return records.ToList();
            }
        }
    }
}