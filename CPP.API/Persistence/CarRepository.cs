using System.Linq;
using System.Globalization;
using System.IO;
using CsvHelper;
using System.Collections.Generic;
using CPP.API.Mapping;
using CPP.API.Core.Models;
using CPP.API.Core;

namespace CPP.API.Persistence
{
    public class CarRepository : ICarRepository
    {
        private readonly string _fileName = "Data/cars.csv";

        public IEnumerable<Car> GetAll()
        {
            using var streamReader = new StreamReader(_fileName);
            using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            csvReader.Context.RegisterClassMap<CarClassMap>();
            return csvReader.GetRecords<Car>().ToList();
        }
    }
}