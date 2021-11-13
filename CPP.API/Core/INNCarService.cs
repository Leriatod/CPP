using CPP.API.Core.Models;

namespace CPP.API.Core
{
    public interface INNCarService
    {
        double PredictPrice(Car car);
        void TrainNN(int epochNumber);
    }
}