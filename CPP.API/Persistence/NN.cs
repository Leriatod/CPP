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
                _weights[l] = new double[layerSizes[l + 1]][];
                _biases[l] = new double[GetLayerSize(l)];

                _deltas[l] = new double[GetLayerSize(l)];
                _layerOutputs[l] = new double[GetLayerSize(l)];
                _layerInputs[l] = new double[GetLayerSize(l)];

                for (int j = 0; j < GetLayerSize(l); j++)
                {
                    _weights[l][j] = new double[layerSizes[l]];
                }
            }

            _optimizer.Initialize(_weights, _biases);
        }

        public void SetRandomCoefficients()
        {
            var random = new Random();
            for (int l = 0; l < LayerNumber; l++)
            {
                for (int j = 0; j < GetLayerSize(l); j++)
                {
                    _biases[l][j] = random.NextDoubleBetween(-1, 1);
                    for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                    {
                        _weights[l][j][i] = random.NextDoubleBetween(-1, 1);
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
                    for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                    {
                        double input = GetInputForLayer(l, i);
                        _layerInputs[l][j] += _weights[l][j][i] * input;
                    }
                    _layerOutputs[l][j] = _activationFunctions[l].Evaluate(_layerInputs[l][j]);
                }
            }
            return _layerOutputs[LastLayerIndex];
        }

        public double Train(double[] inputs, double[] targets)
        {
            double error = BackPropagateError(inputs, targets);
            UpdateCoefficients();
            return error;
        }

        private double BackPropagateError(double[] inputs, double[] targets)
        {
            double error = 0.0;
            for (int l = LastLayerIndex; l >= 0; l--)
            {
                if (l == LastLayerIndex)
                {
                    error = PropagateErrorFromLastLayer(inputs, targets);
                    continue;
                }
                PropagateErrorFromHiddenLayer(l);
            }
            return error;
        }

        private double PropagateErrorFromLastLayer(double[] inputs, double[] targets)
        {
            double error = 0.0;
            double[] outputs = Run(inputs);
            for (int j = 0; j < GetLayerSize(LastLayerIndex); j++)
            {
                double delta = outputs[j] - targets[j];
                _deltas[LastLayerIndex][j] = delta * _activationFunctions[LastLayerIndex].EvaluateDerivative(_layerInputs[LastLayerIndex][j]);
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
                    _deltas[layerIndex][i] += _weights[layerIndex + 1][j][i] * _deltas[layerIndex + 1][j];
                }
                _deltas[layerIndex][i] *= _activationFunctions[layerIndex].EvaluateDerivative(_layerInputs[layerIndex][i]);
            }
        }

        private void UpdateCoefficients()
        {
            for (int l = 0; l < LayerNumber; l++)
            {
                for (int j = 0; j < GetLayerSize(l); j++)
                {
                    double biasGradient = _deltas[l][j];
                    _optimizer.UpdateBias(l, j, biasGradient);

                    for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                    {
                        double weightGradient = _deltas[l][j] * GetInputForLayer(l, i);
                        _optimizer.UpdateWeight(l, j, i, weightGradient);
                    }
                }
            }
        }

        private double GetInputForLayer(int layerIndex, int inputIndex) => layerIndex == 0 ? _inputs[inputIndex] : _layerOutputs[layerIndex - 1][inputIndex];
        private int GetLayerSize(int layerIndex) => _weights[layerIndex].Length;
        private int GetInputsNumberForLayer(int layerIndex) => _weights[layerIndex][0].Length;
    }
}