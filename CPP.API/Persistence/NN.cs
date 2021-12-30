using System;
using CPP.API.Core;
using CPP.API.Extensions;

namespace CPP.API.Persistence
{
    [Serializable]
    public class NN : INN
    {
        private IActivationFunction[] _activationFunctions;
        private INNOptimizer _optimizer;
        private double[][][] _weights;
        private double[][] _biases;
        private double[][] _layerOutputs;
        private double[][] _layerInputs;
        private double[][] _deltas;
        private double[] _inputs;
        private int LayerNumber => _weights.Length;
        private int LastLayerIndex => LayerNumber - 1;

        public void Initialize(int[] layerSizes, IActivationFunction[] activationFunctions, INNOptimizer optimizer)
        {
            _optimizer = optimizer;
            _activationFunctions = activationFunctions;

            _weights = new double[layerSizes.Length - 1][][];
            _biases = new double[LayerNumber][];

            _deltas = new double[LayerNumber][];
            _layerOutputs = new double[LayerNumber][];
            _layerInputs = new double[LayerNumber][];

            for (int l = 0; l < LayerNumber; l++)
            {
                _biases[l] = new double[layerSizes[l + 1]];
                _weights[l] = new double[layerSizes[l]][];

                _deltas[l] = new double[GetLayerSize(l)];
                _layerOutputs[l] = new double[GetLayerSize(l)];
                _layerInputs[l] = new double[GetLayerSize(l)];

                for (int i = 0; i < GetPreviousLayerSize(l); i++)
                {
                    _weights[l][i] = new double[GetLayerSize(l)];
                }
            }
        }

        public void SetRandomCoefficients()
        {
            var random = new Random();
            for (int l = 0; l < LayerNumber; l++)
            {
                for (int j = 0; j < GetLayerSize(l); j++)
                {
                    _biases[l][j] = random.NextDoubleBetween(-1, 1);
                }
                for (int i = 0; i < GetPreviousLayerSize(l); i++)
                {
                    for (int j = 0; j < GetLayerSize(l); j++)
                    {
                        _weights[l][i][j] = random.NextDoubleBetween(-1, 1);
                    }
                }
            }
        }

        public double[] Run(double[] inputs)
        {
            _inputs = inputs;
            for (int l = 0; l < LayerNumber; l++)
            {
                for (int j = 0; j < GetLayerSize(l); j++)
                {
                    _layerInputs[l][j] = _biases[l][j];
                    for (int i = 0; i < GetPreviousLayerSize(l); i++)
                    {
                        double input = GetInputForNeuron(l, i);
                        _layerInputs[l][j] += _weights[l][i][j] * input;
                    }
                    _layerOutputs[l][j] = _activationFunctions[l].Evaluate(_layerInputs[l][j]);
                }
            }
            return _layerOutputs[LastLayerIndex].Clone() as double[];
        }

        public double Train(double[] inputs, double[] targets)
        {
            double error = BackPropagateError(inputs, targets);
            UpdateWeights();
            UpdateBiases();
            return error;
        }

        private double BackPropagateError(double[] inputs, double[] targets)
        {
            double[] outputs = Run(inputs);
            double error = 0.0;
            for (int l = LastLayerIndex; l >= 0; l--)
            {
                if (l == LastLayerIndex)
                {
                    error = PropagateErrorFromLastLayer(targets, outputs);
                    continue;
                }
                PropagateErrorFromHiddenLayer(l);
            }
            return error;
        }

        private double PropagateErrorFromLastLayer(double[] targets, double[] outputs)
        {
            double error = 0.0;
            for (int i = 0; i < GetLayerSize(LastLayerIndex); i++)
            {
                double delta = outputs[i] - targets[i];
                _deltas[LastLayerIndex][i] = delta * _activationFunctions[LastLayerIndex].EvaluateDerivative(_layerInputs[LastLayerIndex][i]);
                error += Math.Pow(delta, 2);
            }
            return error;
        }

        private void PropagateErrorFromHiddenLayer(int layerIndex)
        {
            for (int i = 0; i < GetLayerSize(layerIndex); i++)
            {
                _deltas[layerIndex][i] = 0.0;
                for (int j = 0; j < GetLayerSize(layerIndex + 1); j++)
                {
                    _deltas[layerIndex][i] += _weights[layerIndex + 1][i][j] * _deltas[layerIndex + 1][j];
                }
                _deltas[layerIndex][i] *= _activationFunctions[layerIndex].EvaluateDerivative(_layerInputs[layerIndex][i]);
            }
        }

        private void UpdateWeights()
        {
            for (int l = 0; l < LayerNumber; l++)
            {
                for (int i = 0; i < GetPreviousLayerSize(l); i++)
                {
                    for (int j = 0; j < GetLayerSize(l); j++)
                    {
                        double gradient = _deltas[l][j] * GetInputForNeuron(l, i);
                        _optimizer.UpdateWeight(_weights, l, i, j, gradient);
                    }
                }
            }
        }

        private void UpdateBiases()
        {
            for (int l = 0; l < LayerNumber; l++)
            {
                for (int i = 0; i < GetLayerSize(l); i++)
                {
                    double gradient = _deltas[l][i];
                    _optimizer.UpdateBias(_biases, l, i, gradient);
                }
            }
        }

        private double GetInputForNeuron(int layerIndex, int neuronIndex) => layerIndex == 0 ? _inputs[neuronIndex] : _layerOutputs[layerIndex - 1][neuronIndex];
        private int GetLayerSize(int layerIndex) => _biases[layerIndex].Length;
        private int GetPreviousLayerSize(int layerIndex) => _weights[layerIndex].Length;
    }
}