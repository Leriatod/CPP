using System.Linq;
using System.Globalization;
using System.IO;
using CsvHelper;
using System.Collections.Generic;
using CPP.API.Mapping;
using CPP.API.Core.Models;
using CPP.API.Core;
using CPP.API.Extensions;

namespace CPP.API.Persistence
{
    public class CarReader : ICarReader
    {
        private readonly string _trainDataFileName = "Data/cars_train.csv";
        private readonly string _testDataFileName = "Data/cars_test.csv";

        public IEnumerable<Car> ReadTrainData()
        {
            return ReadDataFromFile(_trainDataFileName);
        }

        public IEnumerable<Car> ReadTestData()
        {
            return ReadDataFromFile(_testDataFileName);
        }

        public CarFeatureCategories ReadCarFeatureCategories()
        {
            var data = ReadTrainData();
            return new CarFeatureCategories()
            {
                Manufacturers = data.SelectOrderedUniqueStrings(d => d.Manufacturer),
                Models = data.SelectOrderedUniqueStrings(d => d.Model),
                Bodies = data.SelectOrderedUniqueStrings(d => d.Body),
                Drives = data.SelectOrderedUniqueStrings(d => d.Drive),
                Transmissions = data.SelectOrderedUniqueStrings(d => d.Transmission),
                Fuels = data.SelectOrderedUniqueStrings(d => d.Fuel)
            };
        }

        private static IEnumerable<Car> ReadDataFromFile(string fileName)
        {
            using var streamReader = new StreamReader(fileName);
            using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            csvReader.Context.RegisterClassMap<CarClassMap>();
            return csvReader.GetRecords<Car>().ToList();
        }
    }
}