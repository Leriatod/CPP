using System.Collections.Generic;
using WebApi.Core;
using WebApi.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
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