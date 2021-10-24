using System.Collections.Generic;
using CPP.API.Core.Models;

namespace CPP.API.Core
{
    public interface ICarRepository
    {
        IEnumerable<Car> GetAll();
    }
}