using System;
using CPP.API.Core;
using CPP.API.Extensions;

namespace CPP.API.Persistence
{
    public class NN : INN
    {
        private IActivationFunction[] _activationFunctions;
        private int _layerNumber;
        private int _inputSize;
        private int[] _nonInputLayerSizes;
        private double[][] _layerOutputs;
        private double[][] _layerInputs;
        private double[][] _biases;
        private double[][] _deltas;
        private double[][][] _weights;
        private double[][] _previousBiasDeltas;
        private double[][][] _previousWeightDeltas;

        public void Initialize(int[] layerSizes, IActivationFunction[] activationFunctions)
        {
            _activationFunctions = activationFunctions;
            _inputSize = layerSizes[0];
            _layerNumber = layerSizes.Length - 1;

            _nonInputLayerSizes = new int[_layerNumber];
            for (int i = 0; i < _layerNumber; i++)
            {
                _nonInputLayerSizes[i] = layerSizes[i + 1];
            }

            _biases = new double[_layerNumber][];
            _previousBiasDeltas = new double[_layerNumber][];
            _deltas = new double[_layerNumber][];
            _layerOutputs = new double[_layerNumber][];
            _layerInputs = new double[_layerNumber][];
            _weights = new double[_layerNumber][][];
            _previousWeightDeltas = new double[_layerNumber][][];
            for (int l = 0; l < _layerNumber; l++)
            {
                _biases[l] = new double[_nonInputLayerSizes[l]];
                _previousBiasDeltas[l] = new double[_nonInputLayerSizes[l]];
                _deltas[l] = new double[_nonInputLayerSizes[l]];
                _layerOutputs[l] = new double[_nonInputLayerSizes[l]];
                _layerInputs[l] = new double[_nonInputLayerSizes[l]];

                _weights[l] = new double[GetInputsNumberForLayer(l)][];
                _previousWeightDeltas[l] = new double[GetInputsNumberForLayer(l)][];
                for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                {
                    _weights[l][i] = new double[_nonInputLayerSizes[l]];
                    _previousWeightDeltas[l][i] = new double[_nonInputLayerSizes[l]];
                }
            }
        }

        public void Set(double[][][] weights, double[][] biases)
        {
            _weights = weights;
            _biases = biases;
        }

        public void SetRandom()
        {
            var random = new Random();
            for (int l = 0; l < _layerNumber; l++)
            {
                for (int j = 0; j < _nonInputLayerSizes[l]; j++)
                {
                    _biases[l][j] = random.NextDoubleBetween(-1, 1);
                }

                for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                {
                    for (int j = 0; j < _nonInputLayerSizes[l]; j++)
                    {
                        _weights[l][i][j] = random.NextDoubleBetween(-1, 1);
                    }
                }
            }
        }

        public double[] Run(double[] inputs)
        {
            for (int l = 0; l < _layerNumber; l++)
            {
                for (int j = 0; j < _nonInputLayerSizes[l]; j++)
                {
                    _layerInputs[l][j] = _biases[l][j];
                    for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                    {
                        double input = GetInputFromLayer(inputs, l, i);
                        _layerInputs[l][j] += _weights[l][i][j] * input;
                    }
                    _layerOutputs[l][j] = _activationFunctions[l].Evaluate(_layerInputs[l][j]);
                }
            }
            return (double[])_layerOutputs[_layerNumber - 1].Clone();
        }

        public double Train(double[] inputs, double[] targets, double learningRate, double momentumRate)
        {
            double error = BackPropagateError(inputs, targets);
            UpdateWeights(inputs, learningRate, momentumRate);
            UpdateBiases(learningRate, momentumRate);
            return error;
        }

        private double BackPropagateError(double[] inputs, double[] targets)
        {
            double[] outputs = Run(inputs);
            double error = 0.0;
            for (int l = _layerNumber - 1; l >= 0; l--)
            {
                if (l == _layerNumber - 1)
                {
                    error = PropagateErrorFromOutputLayer(targets, outputs, l);
                    continue;
                }
                PropagateErrorFromHiddenLayer(l);
            }
            return error;
        }

        private void PropagateErrorFromHiddenLayer(int layerIndex)
        {
            for (int i = 0; i < _nonInputLayerSizes[layerIndex]; i++)
            {
                _deltas[layerIndex][i] = 0.0;
                for (int j = 0; j < _nonInputLayerSizes[layerIndex + 1]; j++)
                {
                    _deltas[layerIndex][i] += _weights[layerIndex + 1][i][j] * _deltas[layerIndex + 1][j];
                }
                _deltas[layerIndex][i] *= _activationFunctions[layerIndex].EvaluateDerivative(_layerInputs[layerIndex][i]);
            }
        }

        private double PropagateErrorFromOutputLayer(double[] targets, double[] outputs, int layerIndex)
        {
            double error = 0.0;
            for (int k = 0; k < _nonInputLayerSizes[layerIndex]; k++)
            {
                double delta = outputs[k] - targets[k];
                _deltas[layerIndex][k] = delta * _activationFunctions[layerIndex].EvaluateDerivative(_layerInputs[layerIndex][k]);
                error += Math.Pow(delta, 2);
            }
            return error;
        }

        private void UpdateWeights(double[] inputs, double learningRate, double momentumRate)
        {
            for (int l = 0; l < _layerNumber; l++)
            {
                for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                {
                    for (int j = 0; j < _nonInputLayerSizes[l]; j++)
                    {
                        double weightDelta = learningRate * _deltas[l][j] * GetInputFromLayer(inputs, l, i) + momentumRate * _previousWeightDeltas[l][i][j];
                        _previousWeightDeltas[l][i][j] = weightDelta;
                        _weights[l][i][j] -= weightDelta;
                    }
                }
            }
        }

        private void UpdateBiases(double learningRate, double momentumRate)
        {
            for (int l = 0; l < _layerNumber; l++)
            {
                for (int i = 0; i < _nonInputLayerSizes[l]; i++)
                {
                    double biasDelta = learningRate * _deltas[l][i] + momentumRate * _previousBiasDeltas[l][i];
                    _previousBiasDeltas[l][i] = biasDelta;
                    _biases[l][i] -= biasDelta;
                }
            }
        }

        private double GetInputFromLayer(double[] inputs, int layerIndex, int inputIndex)
        {
            return layerIndex == 0 ? inputs[inputIndex] : _layerOutputs[layerIndex - 1][inputIndex];
        }

        private int GetInputsNumberForLayer(int layerIndex)
        {
            return layerIndex == 0 ? _inputSize : _nonInputLayerSizes[layerIndex - 1];
        }
    }
}