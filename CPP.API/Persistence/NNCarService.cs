using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CPP.API.Core;
using CPP.API.Core.Models;
using CPP.API.Extensions;
using CPP.API.Heplers;
using CPP.API.Persistence.ActivationFunctions;
using CPP.API.Persistence.Optimizers;

namespace CPP.API.Persistence
{
    public class NNCarService : INNCarService
    {
        private readonly string _nnReadFilePath = "Data/adam.bin";
        private readonly string _nnWriteFilePath = "Data/rmsprop.bin";
        private readonly INN _nn;
        private readonly ICarReader _reader;
        private readonly IEnumerable<Car> _trainData;
        private readonly CarOneHotEncoder _oneHotEncoder;
        private readonly CarStandardScaler _standardScaler;

        public NNCarService(INN nn, ICarReader reader)
        {
            _reader = reader;
            _trainData = _reader.ReadTrainData();
            _oneHotEncoder = new CarOneHotEncoder(_trainData);
            _standardScaler = new CarStandardScaler(_trainData);

            if (File.Exists(_nnReadFilePath))
            {
                _nn = BinaryHelper.ReadFromBinaryFile<INN>(_nnReadFilePath);
                return;
            }

            _nn = nn;
        }

        public double PredictPrice(Car car)
        {
            double[] input = _oneHotEncoder
                .Encode(_standardScaler.Scale(car))
                .RemoveLast();

            return _nn.Run(input)[0];
        }

        public void TrainNN(int epochNumber)
        {
            double[][] data = _oneHotEncoder.EncodeAll(_standardScaler.ScaleAll(_trainData)).Shuffle();
            double[][] inputs = data.RemoveLastColumn();
            double[] targetPrices = data.GetLastColumn();

            InitializeRandomNN(inputs[0].Length);

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

            stopwatch.Stop();

            Console.WriteLine($"Training time: {stopwatch.Elapsed.TotalSeconds} seconds.");

            PrintRSquareWithTestData();

            BinaryHelper.WriteToBinaryFile(_nnWriteFilePath, _nn);
        }

        private void InitializeRandomNN(int inputSize)
        {
            _nn.Initialize(
                new int[] { inputSize, 128, 64, 32, 1 },
                new IActivationFunction[] { new ReLU(), new ReLU(), new ReLU(), new Linear() },
                new RMSpropOptimizer());

            _nn.SetRandomCoefficients();
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