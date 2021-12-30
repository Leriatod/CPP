using System;
using CPP.API.Core;

namespace CPP.API.Persistence.Optimizers
{
    [Serializable]
    public class RMSpropOptimizer : INNOptimizer
    {
        private readonly double _beta = 0.999;
        private readonly double _epsilon = 1e-8;
        private readonly double _learningRate;

        private double[][][] _weightsSquaredGradientEMA;
        private double[][] _biasesSquaredGradientEMA;

        private bool _isWeightsSquaredGradientEMAInitialized = false;
        private bool _isBiasesSquaredGradientEMAInitialized = false;


        public RMSpropOptimizer(double learningRate = 0.001)
        {
            _learningRate = learningRate;
        }

        public void UpdateWeight(double[][][] weights, int layerIndex, int inputIndex, int neuronIndex, double gradient)
        {
            if (!_isWeightsSquaredGradientEMAInitialized)
            {
                InitializeSquaredWeightsGradientEMA(weights);
            }

            _weightsSquaredGradientEMA[layerIndex][inputIndex][neuronIndex] =
                _beta * _weightsSquaredGradientEMA[layerIndex][inputIndex][neuronIndex] + (1 - _beta) * Math.Pow(gradient, 2);

            weights[layerIndex][inputIndex][neuronIndex] -=
                _learningRate * gradient / (Math.Sqrt(_weightsSquaredGradientEMA[layerIndex][inputIndex][neuronIndex]) + _epsilon);
        }

        public void UpdateBias(double[][] biases, int layerIndex, int neuronIndex, double gradient)
        {
            if (!_isBiasesSquaredGradientEMAInitialized)
            {
                InitializeSquaredBiasesGradientEMA(biases);
            }

            _biasesSquaredGradientEMA[layerIndex][neuronIndex] =
                _beta * _biasesSquaredGradientEMA[layerIndex][neuronIndex] + (1 - _beta) * Math.Pow(gradient, 2);

            biases[layerIndex][neuronIndex] -=
                _learningRate * gradient / (Math.Sqrt(_biasesSquaredGradientEMA[layerIndex][neuronIndex]) + _epsilon);
        }

        private void InitializeSquaredWeightsGradientEMA(double[][][] weights)
        {
            int layerNumber = weights.Length;
            _weightsSquaredGradientEMA = new double[layerNumber][][];

            for (int l = 0; l < layerNumber; l++)
            {
                int previousLayerSize = weights[l].Length;
                _weightsSquaredGradientEMA[l] = new double[previousLayerSize][];

                for (int i = 0; i < previousLayerSize; i++)
                {
                    int layerSize = weights[l][i].Length;
                    _weightsSquaredGradientEMA[l][i] = new double[layerSize];
                }
            }
            _isWeightsSquaredGradientEMAInitialized = true;
        }

        private void InitializeSquaredBiasesGradientEMA(double[][] biases)
        {
            int layerNumber = biases.Length;
            _biasesSquaredGradientEMA = new double[layerNumber][];

            for (int l = 0; l < layerNumber; l++)
            {
                int layerSize = biases[l].Length;
                _biasesSquaredGradientEMA[l] = new double[layerSize];
            }
            _isBiasesSquaredGradientEMAInitialized = true;
        }
    }
}