using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using CPP.API.Core;
using CPP.API.Core.Models;
using CPP.API.Extensions;
using CPP.API.Helpers;
using CPP.API.Persistence.ActivationFunctions;
using CPP.API.Persistence.Optimizers;

namespace CPP.API.Persistence
{
    public class NNCarService : INNCarService
    {
        private readonly string _nnReadFilePath = "Data/model.bin";
        private readonly string _nnWriteFilePath = "Data/model2.bin";
        private readonly INN _nn;
        private readonly ICarReader _reader;
        private readonly IEnumerable<Car> _trainData;
        private readonly CarOneHotEncoder _oneHotEncoder;
        private readonly CarStandardScaler _standardScaler;
        private readonly ILogger<NNCarService> _logger;

        public NNCarService(INN nn, ICarReader reader, ILogger<NNCarService> logger)
        {
            _logger = logger;

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
            double[][] data = _oneHotEncoder.EncodeAll(_standardScaler.ScaleAll(_trainData));

            InitializeRandomNN(inputSize: data[0].Length - 1);

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int epochCounter = 0; epochCounter < epochNumber; epochCounter++)
            {
                data = data.Shuffle();
                double[][] inputs = data.RemoveLastColumn();
                double[][] targetPrices = data.TakeLastColumns(1);

                double mse = 0.0;
                double mae = 0.0;
                for (int sampleCounter = 0; sampleCounter < inputs.Length; sampleCounter++)
                {
                    double[] input = inputs[sampleCounter];
                    double[] target = targetPrices[sampleCounter];

                    double error = _nn.Train(input, target);
                    mse += error;
                    mae += Math.Sqrt(error);
                }
                _logger.LogInformation($"Epoch: {epochCounter + 1}, MSE: {mse / inputs.Length}, MAE: {mae / inputs.Length}");
            }

            stopwatch.Stop();

            _logger.LogInformation($"Training time: {stopwatch.Elapsed.TotalSeconds} seconds.");

            LogNNPerformance();

            BinaryHelper.WriteToBinaryFile(_nnWriteFilePath, _nn);
        }

        private void InitializeRandomNN(int inputSize)
        {
            _nn.Initialize(
                new int[] { inputSize, 128, 64, 32, 1 },
                new IActivationFunction[] { new ReLU(), new ReLU(), new ReLU(), new Linear() },
                new AdamOptimizer(),
                l2Lambda: 0.01);

            _nn.SetRandomCoefficients();
        }

        private void LogNNPerformance()
        {
            var testData = _reader.ReadTestData();
            var targetPrices = testData.Select(car => car.Price);
            var predictedPrices = testData.Select(car => PredictPrice(car));

            _logger.LogInformation("Neural network performance on the test data:");
            LogRegressionMetrics(targetPrices, predictedPrices);

            targetPrices = _trainData.Select(car => car.Price);
            predictedPrices = _trainData.Select(car => PredictPrice(car));

            _logger.LogInformation("Neural network performance on the train data:");
            LogRegressionMetrics(targetPrices, predictedPrices);
        }

        private void LogRegressionMetrics(IEnumerable<double> targetPrices, IEnumerable<double> predictedPrices)
        {
            _logger.LogInformation($"Coefficient of determination (R2) = {targetPrices.GetRSquare(predictedPrices)}");
            _logger.LogInformation($"Mean absolute error (MAE) = {targetPrices.GetMAE(predictedPrices)}");
            _logger.LogInformation($"Mean squared error (MSE) = {targetPrices.GetMSE(predictedPrices)}");
            _logger.LogInformation($"Mean absolute percentage error (MAPE) = {100 * targetPrices.GetMAPE(predictedPrices):0.00}%");
        }
    }
}