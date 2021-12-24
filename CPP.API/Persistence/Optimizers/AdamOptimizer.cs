using System;
using CPP.API.Core;

namespace CPP.API.Persistence.Optimizers
{
    public class AdamOptimizer : INNOptimizer
    {
        private readonly double _beta1 = 0.9;
        private readonly double _beta2 = 0.999;
        private readonly double _epsilon = 1e-8;
        private readonly double _learningRate;

        private double[][][] _weightsGradientEMA;
        private double[][][] _weightsSquaredGradientEMA;
        private double[][] _biasesGradientEMA;
        private double[][] _biasesSquaredGradientEMA;

        private bool _isWeightsGradientEMAInitialized = false;
        private bool _isBiasesGradientEMAInitialized = false;

        public AdamOptimizer(double learningRate = 0.001)
        {
            _learningRate = learningRate;
        }

        public void UpdateWeight(double[][][] weights, int layerIndex, int inputIndex, int neuronIndex, double gradient)
        {
            if (!_isWeightsGradientEMAInitialized)
            {
                InitializeWeightsGradientEMA(weights);
            }

            _weightsGradientEMA[layerIndex][inputIndex][neuronIndex] =
                _beta1 * _weightsGradientEMA[layerIndex][inputIndex][neuronIndex] + (1 - _beta1) * gradient;

            _weightsSquaredGradientEMA[layerIndex][inputIndex][neuronIndex] =
                _beta2 * _weightsSquaredGradientEMA[layerIndex][inputIndex][neuronIndex] + (1 - _beta2) * Math.Pow(gradient, 2);

            weights[layerIndex][inputIndex][neuronIndex] -=
                _learningRate * _weightsGradientEMA[layerIndex][inputIndex][neuronIndex] / (Math.Sqrt(_weightsSquaredGradientEMA[layerIndex][inputIndex][neuronIndex]) + _epsilon);
        }

        public void UpdateBias(double[][] biases, int layerIndex, int neuronIndex, double gradient)
        {
            if (!_isBiasesGradientEMAInitialized)
            {
                InitializeBiasesGradientEMA(biases);
            }

            _biasesGradientEMA[layerIndex][neuronIndex] =
                _beta1 * _biasesGradientEMA[layerIndex][neuronIndex] + (1 - _beta1) * gradient;

            _biasesSquaredGradientEMA[layerIndex][neuronIndex] =
                _beta2 * _biasesSquaredGradientEMA[layerIndex][neuronIndex] + (1 - _beta2) * Math.Pow(gradient, 2);

            biases[layerIndex][neuronIndex] -=
                _learningRate * _biasesGradientEMA[layerIndex][neuronIndex] / (Math.Sqrt(_biasesSquaredGradientEMA[layerIndex][neuronIndex]) + _epsilon);
        }

        private void InitializeWeightsGradientEMA(double[][][] weights)
        {
            int layerNumber = weights.Length;

            _weightsGradientEMA = new double[layerNumber][][];
            _weightsSquaredGradientEMA = new double[layerNumber][][];

            for (int l = 0; l < layerNumber; l++)
            {
                int previousLayerSize = weights[l].Length;

                _weightsGradientEMA[l] = new double[previousLayerSize][];
                _weightsSquaredGradientEMA[l] = new double[previousLayerSize][];

                for (int i = 0; i < previousLayerSize; i++)
                {
                    int layerSize = weights[l][i].Length;

                    _weightsGradientEMA[l][i] = new double[layerSize];
                    _weightsSquaredGradientEMA[l][i] = new double[layerSize];
                }
            }
            _isWeightsGradientEMAInitialized = true;
        }

        private void InitializeBiasesGradientEMA(double[][] biases)
        {
            int layerNumber = biases.Length;

            _biasesGradientEMA = new double[layerNumber][];
            _biasesSquaredGradientEMA = new double[layerNumber][];

            for (int l = 0; l < layerNumber; l++)
            {
                int layerSize = biases[l].Length;

                _biasesGradientEMA[l] = new double[layerSize];
                _biasesSquaredGradientEMA[l] = new double[layerSize];
            }
            _isBiasesGradientEMAInitialized = true;
        }
    }
}