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

        private double[][][] _weights;
        private double[][] _biases;
        private double[][][] _weightsSquaredGradientEMA;
        private double[][] _biasesSquaredGradientEMA;

        public RMSpropOptimizer(double learningRate = 0.001)
        {
            _learningRate = learningRate;
        }

        public void Initialize(double[][][] weights, double[][] biases)
        {
            _weights = weights;
            _biases = biases;

            int layerNumber = weights.Length;

            _weightsSquaredGradientEMA = new double[layerNumber][][];
            _biasesSquaredGradientEMA = new double[layerNumber][];

            for (int l = 0; l < layerNumber; l++)
            {
                int layerSize = weights[l].Length;

                _weightsSquaredGradientEMA[l] = new double[layerSize][];
                _biasesSquaredGradientEMA[l] = new double[layerSize];

                for (int j = 0; j < layerSize; j++)
                {
                    int inputsNumber = weights[l][j].Length;
                    _weightsSquaredGradientEMA[l][j] = new double[inputsNumber];
                }
            }
        }

        public void UpdateWeight(int layerIndex, int neuronIndex, int inputIndex, double gradient)
        {
            _weightsSquaredGradientEMA[layerIndex][neuronIndex][inputIndex] =
                _beta * _weightsSquaredGradientEMA[layerIndex][neuronIndex][inputIndex] + (1 - _beta) * Math.Pow(gradient, 2);

            _weights[layerIndex][neuronIndex][inputIndex] -=
                _learningRate * gradient / (Math.Sqrt(_weightsSquaredGradientEMA[layerIndex][neuronIndex][inputIndex]) + _epsilon);
        }

        public void UpdateBias(int layerIndex, int neuronIndex, double gradient)
        {
            _biasesSquaredGradientEMA[layerIndex][neuronIndex] =
                _beta * _biasesSquaredGradientEMA[layerIndex][neuronIndex] + (1 - _beta) * Math.Pow(gradient, 2);

            _biases[layerIndex][neuronIndex] -=
                _learningRate * gradient / (Math.Sqrt(_biasesSquaredGradientEMA[layerIndex][neuronIndex]) + _epsilon);
        }
    }
}