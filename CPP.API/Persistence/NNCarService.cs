using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CPP.API.Core;
using CPP.API.Core.Models;
using CPP.API.Extensions;
using CPP.API.Persistence.ActivationFunctions;
using CPP.API.Persistence.Optimizers;

namespace CPP.API.Persistence
{
    public class NNCarService : INNCarService
    {
        private bool isNNInitialized = false;

        private readonly ICarReader _reader;
        private readonly INN _nn;
        private readonly INNStorage _nnStorage;
        private readonly CarOneHotEncoder _oneHotEncoder;
        private readonly CarStandardScaler _standardScaler;
        private readonly IEnumerable<Car> _trainData;

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
            double[] input = _oneHotEncoder
                .Encode(_standardScaler.Scale(car))
                .RemoveLast();

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
            double[][] data = _oneHotEncoder.EncodeAll(_standardScaler.ScaleAll(_trainData)).Shuffle();
            double[][] inputs = data.RemoveLastColumn();
            double[] targetPrices = data.GetLastColumn();

            ReinitializeNN(inputs[0].Length);
            _nn.SetRandom();

            var stopwatch = new Stopwatch();

            stopwatch.Start();

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
                }
                Console.WriteLine($"Epoch: {epochCounter + 1}, MSE: {mse / inputs.Length}, MAE: {mae / inputs.Length}");
            }

            isNNInitialized = true;
            stopwatch.Stop();
            Console.WriteLine($"Training time: {stopwatch.Elapsed.TotalSeconds} seconds.");
            PrintRSquareWithTestData();

            var nnCoefficients = new NNCoefficients(_nn.Biases, _nn.Weights);
            _nnStorage.Save(nnCoefficients);
        }

        private void ReinitializeNN(int inputSize)
        {
            _nn.Initialize(
                new int[] { inputSize, 128, 64, 32, 1 },
                new IActivationFunction[] { new ReLU(), new ReLU(), new ReLU(), new Linear() },
                new AdamOptimizer());
        }

        private void PrintRSquareWithTestData()
        {
            var testData = _reader.ReadTestData();
            var targetPrices = testData.Select(car => car.Price);
            var predictedPrices = testData.Select(car => PredictPrice(car));

            double rSquare = targetPrices.GetRSquare(predictedPrices);

            Console.WriteLine($"Coefficient of determination (R2) = {rSquare}");
        }
    }
}