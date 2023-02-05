using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CPP.API.Core;
using CPP.API.Core.Models;
using CPP.API.Extensions;
using CPP.API.NeuralNetwork;
using CPP.API.NeuralNetwork.ActivationFunctions;
using CPP.API.NeuralNetwork.Optimizers;

namespace CPP.API.Services
{
    public class NNTrainer : INNTrainer
    {
        private readonly ICarReader _reader;
        private readonly IEnumerable<Car> _trainData;
        private readonly CarOneHotEncoder _oneHotEncoder;
        private readonly CarStandardScaler _standardScaler;
        private readonly INNSerializer _nnSerializer;

        private readonly ILogger<NNTrainer> _logger;

        private INN _nn;

        public NNTrainer(ICarReader reader, INNSerializer nnSerializer, ILogger<NNTrainer> logger)
        {
            _reader = reader;

            _trainData = _reader.ReadTrainData();
            _oneHotEncoder = new CarOneHotEncoder(_trainData);
            _standardScaler = new CarStandardScaler(_trainData);

            _nnSerializer = nnSerializer;
            _logger = logger;
        }

        public void Train(int epochNumber, string nnReadPath, string nnWritePath)
        {
            double[][] data = _oneHotEncoder.EncodeAll(_standardScaler.ScaleAll(_trainData));

            InitializeNN(inputSize: data[0].Length - 1, nnReadPath);

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

            _nnSerializer.Serialize(nnWritePath, _nn);
        }



        private void InitializeNN(int inputSize, string nnReadPath)
        {
            if (File.Exists(nnReadPath))
            {
                _nn = _nnSerializer.Deserialize(nnReadPath);
                return;
            }

            _nn = new NN(
                new int[] { inputSize, 128, 64, 32, 1 },
                new IActivationFunction[] { new ReLU(), new ReLU(), new ReLU(), new Linear() },
                new AdamOptimizer(),
                l2Lambda: 0.01);
        }

        private void LogNNPerformance()
        {
            var targetPrices = _trainData.Select(car => car.Price);
            var predictedPrices = _trainData.Select(car => PredictPrice(car));

            _logger.LogInformation("Neural network performance on the train data:");
            LogRegressionMetrics(targetPrices, predictedPrices);

            var testData = _reader.ReadTestData();
            targetPrices = testData.Select(car => car.Price);
            predictedPrices = testData.Select(car => PredictPrice(car));

            _logger.LogInformation("Neural network performance on the test data:");
            LogRegressionMetrics(targetPrices, predictedPrices);
        }

        private double PredictPrice(Car car)
        {
            double[] input = _oneHotEncoder
                .Encode(_standardScaler.Scale(car))
                .RemoveLast();

            return _nn.Run(input)[0];
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