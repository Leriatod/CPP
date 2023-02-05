using System;
using CPP.API.Core;

namespace CPP.API.NeuralNetwork.Optimizers
{
    [Serializable]
    public class GradientDescentOptimizer : NNOptimizer
    {
        private double[][][] _coefficients;

        public GradientDescentOptimizer(double learningRate = 0.001, double clipValue = 0.0) : base(learningRate, clipValue) { }

        public override void Initialize(double[][][] coefficients)
        {
            _coefficients = coefficients;
        }

        public override void UpdateCoefficient(int layerIndex, int neuronIndex, int inputIndex, double gradient)
        {
            gradient = ClipGradient(gradient);

            _coefficients[layerIndex][neuronIndex][inputIndex] -= _learningRate * gradient;
        }
    }
}