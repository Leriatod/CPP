using System.Linq;
using System.Globalization;
using System.IO;
using CsvHelper;
using System.Collections.Generic;

namespace CPP.Data
{
    public class CarDatabase
    {
        private readonly string fileName = "Data/CarDataset/audi_cars.csv";
        public List<dynamic> GetAll()
        {
            using (var streamReader = new StreamReader(fileName))
            {
                using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    var records = csvReader.GetRecords<dynamic>().ToList();
                    return records;
                }
            }
        }
    }
}