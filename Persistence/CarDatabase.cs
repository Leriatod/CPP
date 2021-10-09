using System.Linq;
using System.Globalization;
using System.IO;
using CsvHelper;
using System.Collections.Generic;
using CPP.Mapping;
using CPP.Core.Models;

namespace CPP.Persistence
{
    public class CarDatabase
    {
        private readonly string fileName = "Data/CarDataset/audi_cars.csv";
        public IEnumerable<Car> GetAll()
        {
            using (var streamReader = new StreamReader(fileName))
            {
                using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    csvReader.Context.RegisterClassMap<CarClassMap>();
                    return csvReader.GetRecords<Car>().ToList();
                }
            }
        }
    }
}