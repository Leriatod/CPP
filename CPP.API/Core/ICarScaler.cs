using System.Collections.Generic;
using CPP.API.Core.Models;

namespace CPP.API.Core
{
    public interface ICarScaler
    {
        void InitializeFrom(IEnumerable<Car> cars);
        Car Scale(Car car);
        IEnumerable<Car> ScaleAll(IEnumerable<Car> cars);
    }
}