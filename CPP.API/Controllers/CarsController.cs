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
        private readonly ICarService _carService;
        private readonly IMapper _mapper;

        public CarsController(ICarReader carReader, ICarService carService, IMapper mapper)
        {
            _carReader = carReader;
            _carService = carService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("predict")]
        public int PredictPrice([FromBody] CarDto carDto)
        {
            var car = _mapper.Map<Car>(carDto);
            return _carService.PredictPrice(car);
        }

        [HttpGet]
        [Route("feature-categories")]
        public CarFeatureCategoriesDto GetCarFeatureCategories()
        {
            var carFeatureCategories = _carReader.ReadCarFeatureCategories();
            return _mapper.Map<CarFeatureCategoriesDto>(carFeatureCategories);
        }
    }
}