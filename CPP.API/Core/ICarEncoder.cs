using System.Collections.Generic;
using CPP.API.Core.Models;

namespace CPP.API.Core
{
    public interface ICarEncoder
    {
        void InitializeFrom(IEnumerable<Car> cars);
        double[] Encode(Car car);
        double[][] EncodeAll(IEnumerable<Car> cars);
    }
}