using CPP.API.Core;
using CPP.API.Core.Models;
using CPP.API.Extensions;
using CPP.API.Helpers;

namespace CPP.API.Services
{
    public class CarService : ICarService
    {
        private readonly string _nnReadFilePath = "Data/model.bin";
        private readonly INN _nn;
        private readonly CarOneHotEncoder _oneHotEncoder;
        private readonly CarStandardScaler _standardScaler;

        public CarService(ICarReader reader)
        {
            var trainData = reader.ReadTrainData();

            _oneHotEncoder = new CarOneHotEncoder(trainData);
            _standardScaler = new CarStandardScaler(trainData);

            _nn = BinaryHelper.ReadFromBinaryFile<INN>(_nnReadFilePath);
        }

        public int PredictPrice(Car car)
        {
            double[] input = _oneHotEncoder
                .Encode(_standardScaler.Scale(car))
                .RemoveLast();

            return (int)_nn.Run(input)[0];
        }
    }
}