using System;
using CPP.API.Core;

namespace CPP.API.Persistence.Optimizers
{
    [Serializable]
    public class MomentumOptimizer : INNOptimizer
    {
        private readonly double _learningRate;
        private readonly double _momentumRate;

        private double[][][] _weights;
        private double[][] _biases;
        private double[][][] _previousWeightsUpdates;
        private double[][] _previousBiasesUpdates;

        public MomentumOptimizer(double learningRate = 0.001, double momentumRate = 0.9)
        {
            _learningRate = learningRate;
            _momentumRate = momentumRate;
        }

        public void Initialize(double[][][] weights, double[][] biases)
        {
            _weights = weights;
            _biases = biases;

            int layerNumber = weights.Length;

            _previousWeightsUpdates = new double[layerNumber][][];
            _previousBiasesUpdates = new double[layerNumber][];

            for (int l = 0; l < layerNumber; l++)
            {
                int layerSize = weights[l].Length;

                _previousWeightsUpdates[l] = new double[layerSize][];
                _previousBiasesUpdates[l] = new double[layerSize];

                for (int j = 0; j < layerSize; j++)
                {
                    int inputsNumber = weights[l][j].Length;
                    _previousWeightsUpdates[l][j] = new double[inputsNumber];
                }
            }
        }

        public void UpdateWeight(int layerIndex, int neuronIndex, int inputIndex, double gradient)
        {
            gradient = GetClippedGradient(gradient);

            double weightUpdate = _momentumRate * _previousWeightsUpdates[layerIndex][neuronIndex][inputIndex] + _learningRate * gradient;

            _weights[layerIndex][neuronIndex][inputIndex] -= weightUpdate;

            _previousWeightsUpdates[layerIndex][neuronIndex][inputIndex] = weightUpdate;
        }

        public void UpdateBias(int layerIndex, int neuronIndex, double gradient)
        {
            gradient = GetClippedGradient(gradient);

            double biasUpdate = _momentumRate * _previousBiasesUpdates[layerIndex][neuronIndex] + _learningRate * gradient;

            _biases[layerIndex][neuronIndex] -= biasUpdate;

            _previousBiasesUpdates[layerIndex][neuronIndex] = biasUpdate;
        }

        // to avoid exploding gradient problem
        private static double GetClippedGradient(double gradient, double threshold = 1.0)
        {
            if (gradient > threshold) return threshold;
            if (gradient < -threshold) return -threshold;
            return gradient;
        }
    }
}