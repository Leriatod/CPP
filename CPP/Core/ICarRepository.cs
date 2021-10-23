using System.Collections.Generic;
using CPP.Core.Models;

namespace CPP.Core
{
    public interface ICarRepository
    {
        IEnumerable<Car> GetAll();
    }
}