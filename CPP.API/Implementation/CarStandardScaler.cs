using System.Collections.Generic;
using CPP.API.Core.Models;
using System.Linq;
using CPP.API.Extensions;

namespace CPP.API.Implementation
{
    public class CarStandardScaler
    {
        private readonly double _meanEngineCapacity;
        private readonly double _meanHorsepower;
        private readonly double _meanMileage;
        private readonly double _meanManufactureYear;
        private readonly double _stdDevEngineCapacity;
        private readonly double _stdDevHorsepower;
        private readonly double _stdDevMileage;
        private readonly double _stdDevManufactureYear;

        public CarStandardScaler(IEnumerable<Car> cars)
        {
            _meanEngineCapacity = cars.Average(c => c.EngineCapacity);
            _meanHorsepower = cars.Average(c => c.Horsepower);
            _meanMileage = cars.Average(c => c.Mileage);
            _meanManufactureYear = cars.Average(c => c.ManufactureYear);
            _stdDevEngineCapacity = cars.Select(c => c.EngineCapacity).GetStdDev();
            _stdDevHorsepower = cars.Select(c => c.Horsepower).GetStdDev();
            _stdDevMileage = cars.Select(c => c.Mileage).GetStdDev();
            _stdDevManufactureYear = cars.Select(c => (double)c.ManufactureYear).GetStdDev();
        }

        public Car Scale(Car car)
        {
            return new Car()
            {
                Model = car.Model,
                Body = car.Body,
                Drive = car.Drive,
                Transmission = car.Transmission,
                Fuel = car.Fuel,
                EngineCapacity = (car.EngineCapacity - _meanEngineCapacity) / _stdDevEngineCapacity,
                Horsepower = (car.Horsepower - _meanHorsepower) / _stdDevHorsepower,
                Mileage = (car.Mileage - _meanMileage) / _stdDevMileage,
                ManufactureYear = (car.ManufactureYear - _meanManufactureYear) / _stdDevManufactureYear,
                Price = car.Price
            };
        }

        public IEnumerable<Car> ScaleAll(IEnumerable<Car> cars)
        {
            return cars.Select(c => Scale(c));
        }
    }
}
