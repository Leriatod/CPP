using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CPP.API.Core;
using CPP.API.Core.Models;

namespace CPP.API.Controllers
{
    [Route("/api/cars")]
    public class CarsController : ControllerBase
    {
        private readonly ICarReader _carReader;
        private readonly INNCarService _carService;

        public CarsController(ICarReader carReader, INNCarService carService)
        {
            _carService = carService;
            _carReader = carReader;
        }

        public IEnumerable<Car> GetAll()
        {
            return _carReader.ReadTestData();
        }

        [HttpGet]
        [Route("predict")]
        public int GetPriceForCar([FromBody] Car car)
        {
            return (int)_carService.PredictPrice(car);
        }

        [HttpGet]
        [Route("feature-categories")]
        public CarFeatureCategories GetCarFeatureCategories()
        {
            return _carReader.ReadCarFeatureCategories();
        }
    }
}