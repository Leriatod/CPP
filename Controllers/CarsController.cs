using System.Collections.Generic;
using CPP.Core;
using CPP.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CPP.Controllers
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