using System.Linq;
using System.Globalization;
using System.IO;
using CsvHelper;
using System.Collections.Generic;
using CPP.Mapping;
using CPP.Core.Models;
using CPP.Core;

namespace CPP.Persistence
{
    public class CarRepository : ICarRepository
    {
        private readonly string _fileName = "Persistence/CarDataset/audi_cars.csv";
        public IEnumerable<Car> GetAll()
        {
            using (var streamReader = new StreamReader(_fileName))
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