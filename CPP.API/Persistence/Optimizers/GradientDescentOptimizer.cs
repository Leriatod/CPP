using System;
using CPP.API.Core;

namespace CPP.API.Persistence.Optimizers
{
    [Serializable]
    public class GradientDescentOptimizer : INNOptimizer
    {
        private readonly double _learningRate;
        private readonly double _clipValue;

        private double[][][] _weights;
        private double[][] _biases;

        public GradientDescentOptimizer(double learningRate = 0.001, double clipValue = 0.0)
        {
            _clipValue = clipValue;
            _learningRate = learningRate;
        }

        public void Initialize(double[][][] weights, double[][] biases)
        {
            _weights = weights;
            _biases = biases;
        }

        public void UpdateBias(int layerIndex, int neuronIndex, double gradient)
        {
            gradient = ClipGradient(gradient);
            _biases[layerIndex][neuronIndex] -= _learningRate * gradient;
        }

        public void UpdateWeight(int layerIndex, int neuronIndex, int inputIndex, double gradient)
        {
            gradient = ClipGradient(gradient);
            _weights[layerIndex][neuronIndex][inputIndex] -= _learningRate * gradient;
        }

        // to avoid exploding gradient problem, if _clipValue is 0 - no gradient clipping
        private double ClipGradient(double gradient)
        {
            if (_clipValue == 0.0) return gradient;
            if (gradient > _clipValue) return _clipValue;
            if (gradient < -_clipValue) return -_clipValue;
            return gradient;
        }
    }
}