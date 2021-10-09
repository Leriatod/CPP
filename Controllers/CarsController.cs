using System.Collections.Generic;
using CPP.Core.Models;
using CPP.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace CPP.Controllers
{
    [Route("/api/cars")]
    public class CarsController : ControllerBase
    {
        private readonly CarDatabase _db;
        public CarsController(CarDatabase db)
        {
            _db = db;
        }

        [HttpGet]
        public IEnumerable<Car> GetAll()
        {
            return _db.GetAll();
        }
    }
}