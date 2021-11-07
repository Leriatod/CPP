using System.Collections.Generic;
using CPP.API.Core.Models;

namespace CPP.API.Core
{
    public interface ICarReader
    {
        IEnumerable<Car> ReadTrainData();
        IEnumerable<Car> ReadTestData();

    }
}