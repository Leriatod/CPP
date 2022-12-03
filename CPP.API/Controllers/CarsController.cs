using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CPP.API.Core;
using CPP.API.Core.Models;
using CPP.API.Controllers.Dtos;

namespace CPP.API.Controllers
{
    [Route("/api/cars")]
    public class CarsController : ControllerBase
    {
        private readonly ICarReader _carReader;
        private readonly INNCarService _carService;
        private readonly IMapper _mapper;

        public CarsController(ICarReader carReader, INNCarService carService, IMapper mapper)
        {
            _carReader = carReader;
            _carService = carService;
            _mapper = mapper;
        }

        public IEnumerable<Car> GetAll()
        {
            return _carReader.ReadTestData();
        }

        [HttpPost]
        [Route("predict")]
        public int PredictPrice([FromBody] CarDto carDto)
        {
            var car = _mapper.Map<Car>(carDto);
            return (int)_carService.PredictPrice(car);
        }

        [HttpGet]
        [Route("feature-categories")]
        public CarFeatureCategories GetCarFeatureCategories()
        {
            return _carReader.ReadCarFeatureCategories();
        }

        // UNCOMMENT TO TRAIN THE NEURAL NETWORK
        // [HttpGet]
        // [Route("train/{epochs}")]
        // public void Train(int epochs)
        // {
        //     _carService.TrainNN(epochs);
        // }
    }
}