using System.Collections.Generic;
using CPP.API.Core.Models;
using System.Linq;
using CPP.API.Extensions;

namespace CPP.API.Persistence
{
    public class CarStandardScaler
    {
        private readonly double _meanEngine;
        private readonly double _meanHorsepower;
        private readonly double _meanDistance;
        private readonly double _meanYear;
        private readonly double _stdDevEngine;
        private readonly double _stdDevHorsepower;
        private readonly double _stdDevDistance;
        private readonly double _stdDevYear;

        public CarStandardScaler(IEnumerable<Car> cars)
        {
            _meanEngine = cars.Average(c => c.Engine);
            _meanHorsepower = cars.Average(c => c.Horsepower);
            _meanDistance = cars.Average(c => c.Distance);
            _meanYear = cars.Average(c => c.Year);
            _stdDevEngine = cars.Select(c => c.Engine).StdDev();
            _stdDevHorsepower = cars.Select(c => c.Horsepower).StdDev();
            _stdDevDistance = cars.Select(c => c.Distance).StdDev();
            _stdDevYear = cars.Select(c => (double)c.Year).StdDev();
        }

        public Car Scale(Car car)
        {
            return new Car()
            {
                Producer = car.Producer,
                Model = car.Model,
                Body = car.Body,
                Drive = car.Drive,
                Transmission = car.Transmission,
                Fuel = car.Fuel,
                Engine = (car.Engine - _meanEngine) / _stdDevEngine,
                Horsepower = (car.Horsepower - _meanHorsepower) / _stdDevHorsepower,
                Distance = (car.Distance - _meanDistance) / _stdDevDistance,
                Year = (car.Year - _meanYear) / _stdDevYear,
                Price = car.Price
            };
        }

        public IEnumerable<Car> ScaleAll(IEnumerable<Car> cars)
        {
            return cars.Select(c => Scale(c));
        }
    }
}
