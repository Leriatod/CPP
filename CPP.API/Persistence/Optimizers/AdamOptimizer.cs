using System;
using CPP.API.Core;

namespace CPP.API.Persistence.Optimizers
{
    [Serializable]
    public class AdamOptimizer : INNOptimizer
    {
        private readonly double _beta1 = 0.9;
        private readonly double _beta2 = 0.999;
        private readonly double _epsilon = 1e-8;
        private readonly double _learningRate;

        private double[][][] _weights;
        private double[][] _biases;
        private double[][][] _weightsGradientEMA;
        private double[][][] _weightsSquaredGradientEMA;
        private double[][] _biasesGradientEMA;
        private double[][] _biasesSquaredGradientEMA;

        public AdamOptimizer(double learningRate = 0.001)
        {
            _learningRate = learningRate;
        }

        public void Initialize(double[][][] weights, double[][] biases)
        {
            _weights = weights;
            _biases = biases;

            int layerNumber = weights.Length;

            _weightsGradientEMA = new double[layerNumber][][];
            _weightsSquaredGradientEMA = new double[layerNumber][][];
            _biasesSquaredGradientEMA = new double[layerNumber][];
            _biasesGradientEMA = new double[layerNumber][];

            for (int l = 0; l < layerNumber; l++)
            {
                int layerSize = weights[l].Length;

                _weightsSquaredGradientEMA[l] = new double[layerSize][];
                _weightsGradientEMA[l] = new double[layerSize][];
                _biasesSquaredGradientEMA[l] = new double[layerSize];
                _biasesGradientEMA[l] = new double[layerSize];

                for (int j = 0; j < layerSize; j++)
                {
                    int inputsNumber = weights[l][j].Length;

                    _weightsSquaredGradientEMA[l][j] = new double[inputsNumber];
                    _weightsGradientEMA[l][j] = new double[inputsNumber];
                }
            }
        }

        public void UpdateWeight(int layerIndex, int neuronIndex, int inputIndex, double gradient)
        {
            _weightsGradientEMA[layerIndex][neuronIndex][inputIndex] =
                _beta1 * _weightsGradientEMA[layerIndex][neuronIndex][inputIndex] + (1 - _beta1) * gradient;

            _weightsSquaredGradientEMA[layerIndex][neuronIndex][inputIndex] =
                _beta2 * _weightsSquaredGradientEMA[layerIndex][neuronIndex][inputIndex] + (1 - _beta2) * Math.Pow(gradient, 2);

            _weights[layerIndex][neuronIndex][inputIndex] -=
                _learningRate * _weightsGradientEMA[layerIndex][neuronIndex][inputIndex] / (Math.Sqrt(_weightsSquaredGradientEMA[layerIndex][neuronIndex][inputIndex]) + _epsilon);
        }

        public void UpdateBias(int layerIndex, int neuronIndex, double gradient)
        {
            _biasesGradientEMA[layerIndex][neuronIndex] =
                _beta1 * _biasesGradientEMA[layerIndex][neuronIndex] + (1 - _beta1) * gradient;

            _biasesSquaredGradientEMA[layerIndex][neuronIndex] =
                _beta2 * _biasesSquaredGradientEMA[layerIndex][neuronIndex] + (1 - _beta2) * Math.Pow(gradient, 2);

            _biases[layerIndex][neuronIndex] -=
                _learningRate * _biasesGradientEMA[layerIndex][neuronIndex] / (Math.Sqrt(_biasesSquaredGradientEMA[layerIndex][neuronIndex]) + _epsilon);
        }
    }
}