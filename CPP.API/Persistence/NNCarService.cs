using System;
using System.Collections.Generic;
using CPP.API.Core;
using CPP.API.Core.Models;
using CPP.API.Extensions;
using CPP.API.Persistence.ActivationFunctions;

namespace CPP.API.Persistence
{
    public class NNCarService
    {
        private bool isNNInitialized = false;

        private readonly ICarReader _reader;
        private readonly INN _nn;
        private readonly INNStorage _nnStorage;
        private readonly IEnumerable<Car> _trainData;
        private readonly CarOneHotEncoder _oneHotEncoder;
        private readonly CarStandardScaler _standardScaler;

        public NNCarService(INN nn, INNStorage nnStorage, ICarReader reader)
        {
            _reader = reader;
            _trainData = _reader.ReadTrainData();

            _oneHotEncoder = new CarOneHotEncoder(_trainData);
            _standardScaler = new CarStandardScaler(_trainData);

            _nn = nn;
            _nnStorage = nnStorage;
        }

        public double PredictPrice(Car car)
        {
            double[] input = _oneHotEncoder.Encode(_standardScaler.Scale(car));

            if (!isNNInitialized)
            {
                ReinitializeNN(input.Length);

                var nnCoefficients = _nnStorage.Load();
                _nn.Set(nnCoefficients.Weights, nnCoefficients.Biases);

                isNNInitialized = true;
            }

            return _nn.Run(input)[0];
        }

        public void TrainNN(int epochNumber)
        {
            double[][] data = _oneHotEncoder.EncodeAll(_standardScaler.ScaleAll(_trainData));
            double[][] inputs = data.RemoveLastColumn();
            double[] targetPrices = data.GetLastColumn();

            ReinitializeNN(inputs[0].Length);

            _nn.SetRandom();

            for (int epochCounter = 0; epochCounter < epochNumber; epochCounter++)
            {
                double mse = 0.0;
                double mae = 0.0;
                for (int sampleCounter = 0; sampleCounter < inputs.Length; sampleCounter++)
                {
                    double[] input = inputs[sampleCounter];
                    double[] target = new double[] { targetPrices[sampleCounter] };

                    double error = _nn.Train(input, target);
                    mse += error;
                    mae += Math.Sqrt(error);

                    if (sampleCounter % 2500 == 1) Console.WriteLine($"MAE = {mae / sampleCounter}");
                }
                Console.WriteLine($"Epoch: {epochCounter + 1}, MSE: {mse / inputs.Length}, MAE: {mae / inputs.Length}");
            }

            var nnCoefficients = new NNCoefficients(_nn.Biases, _nn.Weights);
            _nnStorage.Save(nnCoefficients);
        }

        private void ReinitializeNN(int inputSize)
        {
            _nn.Initialize(
                new int[] { inputSize, 256, 128, 64, 1 },
                new IActivationFunction[] { new ReLU(), new ReLU(), new ReLU(), new Linear() });
        }
    }
}