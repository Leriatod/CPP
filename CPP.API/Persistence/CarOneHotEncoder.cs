using System;
using System.Linq;
using System.Collections.Generic;
using CPP.API.Core.Models;
using CPP.API.Extensions;

namespace CPP.API.Persistence
{
    public class CarOneHotEncoder
    {
        private readonly Dictionary<string, double[]> _vectorByManufacturer;
        private readonly Dictionary<string, double[]> _vectorByModel;
        private readonly Dictionary<string, double[]> _vectorByBody;
        private readonly Dictionary<string, double[]> _vectorByDrive;
        private readonly Dictionary<string, double[]> _vectorByTransmission;
        private readonly Dictionary<string, double[]> _vectorByFuel;

        public CarOneHotEncoder(IEnumerable<Car> cars)
        {
            _vectorByManufacturer = GetDictionaryEncoderForColumn(c => c.Manufacturer, cars);
            _vectorByModel = GetDictionaryEncoderForColumn(c => c.Model, cars);
            _vectorByBody = GetDictionaryEncoderForColumn(c => c.Body, cars);
            _vectorByDrive = GetDictionaryEncoderForColumn(c => c.Drive, cars);
            _vectorByTransmission = GetDictionaryEncoderForColumn(c => c.Transmission, cars);
            _vectorByFuel = GetDictionaryEncoderForColumn(c => c.Fuel, cars);
        }

        private static Dictionary<string, double[]> GetDictionaryEncoderForColumn(
            Func<Car, string> getCategoricalColumnValue,
            IEnumerable<Car> cars)
        {
            var uniqueCategories = cars
                .SelectOrderedUniqueStrings(getCategoricalColumnValue)
                .ToList();

            var vectorByCategory = new Dictionary<string, double[]>();

            for (int i = 0; i < uniqueCategories.Count; i++)
            {
                var vector = new double[uniqueCategories.Count];
                vector[i] = 1;

                var category = uniqueCategories[i];

                vectorByCategory[category] = vector;
            }

            return vectorByCategory;
        }

        public double[][] EncodeAll(IEnumerable<Car> cars)
        {
            return cars.Select(c => Encode(c)).ToArray();
        }

        public double[] Encode(Car car)
        {
            return GetVectorByColumnValue(_vectorByManufacturer, car.Manufacturer)
                .Concat(GetVectorByColumnValue(_vectorByModel, car.Model))
                .Concat(GetVectorByColumnValue(_vectorByBody, car.Body))
                .Concat(GetVectorByColumnValue(_vectorByDrive, car.Drive))
                .Concat(GetVectorByColumnValue(_vectorByTransmission, car.Transmission))
                .Concat(GetVectorByColumnValue(_vectorByFuel, car.Fuel))
                .Concat(new double[] { car.EngineCapacity })
                .Concat(new double[] { car.Horsepower })
                .Concat(new double[] { car.Mileage })
                .Concat(new double[] { car.ManufactureYear })
                .Concat(new double[] { car.Price })
                .ToArray();
        }

        private static double[] GetVectorByColumnValue(Dictionary<string, double[]> vectorByCategory, string categoryKey)
        {
            return !string.IsNullOrEmpty(categoryKey) && vectorByCategory.ContainsKey(categoryKey) ?
                vectorByCategory[categoryKey]
                : new double[vectorByCategory.Count];
        }
    }
}