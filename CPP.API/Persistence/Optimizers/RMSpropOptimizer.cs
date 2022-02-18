using System;
using CPP.API.Core;

namespace CPP.API.Persistence.Optimizers
{
    [Serializable]
    public class RMSpropOptimizer : NNOptimizer
    {
        private readonly double _beta = 0.999;
        private readonly double _epsilon = 1e-8;

        private double[][][] _coefficients;
        private double[][][] _coefficientsSquaredGradientEMA;

        public RMSpropOptimizer(double learningRate = 0.001, double clipValue = 0.0) : base(learningRate, clipValue) { }

        public override void Initialize(double[][][] coefficients)
        {
            _coefficients = coefficients;

            int layerNumber = coefficients.Length;
            _coefficientsSquaredGradientEMA = new double[layerNumber][][];

            for (int l = 0; l < layerNumber; l++)
            {
                int layerSize = coefficients[l].Length;
                _coefficientsSquaredGradientEMA[l] = new double[layerSize][];

                for (int j = 0; j < layerSize; j++)
                {
                    int inputsNumber = coefficients[l][j].Length;
                    _coefficientsSquaredGradientEMA[l][j] = new double[inputsNumber];
                }
            }
        }

        public override void UpdateCoefficient(int layerIndex, int neuronIndex, int inputIndex, double gradient)
        {
            gradient = ClipGradient(gradient);

            _coefficientsSquaredGradientEMA[layerIndex][neuronIndex][inputIndex] =
                _beta * _coefficientsSquaredGradientEMA[layerIndex][neuronIndex][inputIndex] + (1 - _beta) * Math.Pow(gradient, 2);

            _coefficients[layerIndex][neuronIndex][inputIndex] -=
                _learningRate * gradient / (Math.Sqrt(_coefficientsSquaredGradientEMA[layerIndex][neuronIndex][inputIndex]) + _epsilon);
        }
    }
}