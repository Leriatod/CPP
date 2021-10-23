using System.Collections.Generic;
using WebApi.Core.Models;

namespace WebApi.Core
{
    public interface ICarRepository
    {
        IEnumerable<Car> GetAll();
    }
}