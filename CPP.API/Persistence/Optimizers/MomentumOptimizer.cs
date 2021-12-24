using System;
using CPP.API.Core;

namespace CPP.API.Persistence.Optimizers
{
    public class MomentumOptimizer : INNOptimizer
    {
        private readonly double _learningRate;
        private readonly double _beta = 0.9;

        private double[][][] _previousWeightsUpdates;
        private double[][] _previousBiasesUpdates;
        private bool _isPreviousWeightsUpdatesInitialized = false;
        private bool _isPreviousBiasesUpdatesInitialized = false;

        public MomentumOptimizer(double learningRate = 0.001)
        {
            _learningRate = learningRate;
        }

        public void UpdateWeight(double[][][] weights, int layerIndex, int inputIndex, int neuronIndex, double gradient)
        {
            if (!_isPreviousWeightsUpdatesInitialized)
            {
                InitializePreviousWeightsUpdates(weights);
            }

            gradient = GetClippedGradient(gradient);

            double weightUpdate = _beta * _previousWeightsUpdates[layerIndex][inputIndex][neuronIndex] + _learningRate * gradient;

            weights[layerIndex][inputIndex][neuronIndex] -= weightUpdate;

            _previousWeightsUpdates[layerIndex][inputIndex][neuronIndex] = weightUpdate;
        }

        public void UpdateBias(double[][] biases, int layerIndex, int neuronIndex, double gradient)
        {
            if (!_isPreviousBiasesUpdatesInitialized)
            {
                InitializePreviousBiasesUpdates(biases);
            }

            gradient = GetClippedGradient(gradient);

            double biasUpdate = _beta * _previousBiasesUpdates[layerIndex][neuronIndex] + _learningRate * gradient;

            biases[layerIndex][neuronIndex] -= biasUpdate;

            _previousBiasesUpdates[layerIndex][neuronIndex] = biasUpdate;
        }

        // to avoid exploding gradient problem
        private static double GetClippedGradient(double gradient, double threshold = 1.0)
        {
            if (gradient > threshold) return threshold;
            if (gradient < -threshold) return -threshold;
            return gradient;
        }

        private void InitializePreviousWeightsUpdates(double[][][] weights)
        {
            int layerNumber = weights.Length;
            _previousWeightsUpdates = new double[layerNumber][][];

            for (int l = 0; l < layerNumber; l++)
            {
                int previousLayerSize = weights[l].Length;
                _previousWeightsUpdates[l] = new double[previousLayerSize][];

                for (int i = 0; i < previousLayerSize; i++)
                {
                    int layerSize = weights[l][i].Length;
                    _previousWeightsUpdates[l][i] = new double[layerSize];
                }
            }
            _isPreviousWeightsUpdatesInitialized = true;
        }

        private void InitializePreviousBiasesUpdates(double[][] biases)
        {
            int layerNumber = biases.Length;
            _previousBiasesUpdates = new double[layerNumber][];

            for (int l = 0; l < layerNumber; l++)
            {
                int layerSize = biases[l].Length;
                _previousBiasesUpdates[l] = new double[layerSize];
            }
            _isPreviousBiasesUpdatesInitialized = true;
        }
    }
}