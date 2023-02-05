using System;
using CPP.API.Core;

namespace CPP.API.NeuralNetwork.Optimizers
{
    [Serializable]
    public class MomentumOptimizer : NNOptimizer
    {
        private readonly double _momentumRate;

        private double[][][] _coefficients;
        private double[][][] _previousCoefficientsUpdates;

        public MomentumOptimizer(double learningRate = 0.001, double momentumRate = 0.9, double clipValue = 0.0) : base(learningRate, clipValue)
        {
            _momentumRate = momentumRate;
        }

        public override void Initialize(double[][][] coefficients)
        {
            _coefficients = coefficients;

            int layerNumber = coefficients.Length;
            _previousCoefficientsUpdates = new double[layerNumber][][];

            for (int l = 0; l < layerNumber; l++)
            {
                int layerSize = coefficients[l].Length;
                _previousCoefficientsUpdates[l] = new double[layerSize][];

                for (int j = 0; j < layerSize; j++)
                {
                    int inputsNumber = coefficients[l][j].Length;
                    _previousCoefficientsUpdates[l][j] = new double[inputsNumber];
                }
            }
        }

        public override void UpdateCoefficient(int layerIndex, int neuronIndex, int inputIndex, double gradient)
        {
            gradient = ClipGradient(gradient);

            double coefficientUpdate = _momentumRate * _previousCoefficientsUpdates[layerIndex][neuronIndex][inputIndex] + _learningRate * gradient;

            _coefficients[layerIndex][neuronIndex][inputIndex] -= coefficientUpdate;

            _previousCoefficientsUpdates[layerIndex][neuronIndex][inputIndex] = coefficientUpdate;
        }
    }
}