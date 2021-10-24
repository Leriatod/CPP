using System.Collections.Generic;
using CPP.API.Core;
using CPP.API.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CPP.API.Controllers
{
    [Route("/api/cars")]
    public class CarsController : ControllerBase
    {
        private readonly ICarRepository _repository;

        public CarsController(ICarRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<Car> GetAll()
        {
            return _repository.GetAll();
        }
    }
}