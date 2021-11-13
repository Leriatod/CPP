using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CPP.API.Core;
using CPP.API.Core.Models;
using CPP.API.Persistence;

namespace CPP.API.Controllers
{
    [Route("/api/cars")]
    public class CarsController : ControllerBase
    {
        private readonly ICarReader _carReader;
        private readonly NNCarService _carService;

        public CarsController(ICarReader carReader, NNCarService carService)
        {
            _carService = carService;
            _carReader = carReader;
        }

        [HttpGet]
        public IEnumerable<Car> GetAll()
        {
            return _carReader.ReadTrainData();
        }

        [HttpGet]
        [Route("feature-categories")]
        public CarFeatureCategories GetCarFeatureCategories()
        {
            return _carReader.ReadCarFeatureCategories();
        }
    }
}