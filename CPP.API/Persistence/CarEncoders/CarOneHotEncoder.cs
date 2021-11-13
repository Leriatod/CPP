using System;
using System.Linq;
using System.Collections.Generic;
using CPP.API.Core.Models;
using CPP.API.Core;
using CPP.API.Extensions;

namespace CPP.API.Persistence.CarEncoders
{
    public class CarOneHotEncoder : ICarEncoder
    {
        private Dictionary<string, double[]> _vectorByProducer;
        private Dictionary<string, double[]> _vectorByModel;
        private Dictionary<string, double[]> _vectorByBody;
        private Dictionary<string, double[]> _vectorByDrive;
        private Dictionary<string, double[]> _vectorByTransmission;
        private Dictionary<string, double[]> _vectorByFuel;

        public void InitializeFrom(IEnumerable<Car> cars)
        {
            _vectorByProducer = GetDictionaryEncoderForColumn(c => c.Producer, cars);
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
            return GetVectorByColumnValue(_vectorByProducer, car.Producer)
                .Concat(GetVectorByColumnValue(_vectorByModel, car.Model))
                .Concat(GetVectorByColumnValue(_vectorByBody, car.Body))
                .Concat(GetVectorByColumnValue(_vectorByDrive, car.Drive))
                .Concat(GetVectorByColumnValue(_vectorByTransmission, car.Transmission))
                .Concat(GetVectorByColumnValue(_vectorByFuel, car.Fuel))
                .Concat(new double[] { car.Engine })
                .Concat(new double[] { car.Horsepower })
                .Concat(new double[] { car.Distance })
                .Concat(new double[] { car.Year })
                .Concat(new double[] { car.Price })
                .ToArray();
        }

        private static double[] GetVectorByColumnValue(Dictionary<string, double[]> vectorByCategory, string categoryKey)
        {
            return vectorByCategory.ContainsKey(categoryKey) ?
                vectorByCategory[categoryKey]
                : new double[vectorByCategory.Count];
        }
    }
}