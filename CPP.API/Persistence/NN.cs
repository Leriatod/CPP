using System;
using CPP.API.Core;
using CPP.API.Extensions;

namespace CPP.API.Persistence
{
    public class NNRMSprop : INN
    {
        private readonly double _beta = 0.9;
        private readonly double _learningRate = 0.001;
        private readonly double _epsilon = 1e-7;

        private IActivationFunction[] _activationFunctions;
        private int _layerNumber;
        private int _inputSize;
        private int[] _nonInputLayerSizes;
        private double[][] _layerOutputs;
        private double[][] _layerInputs;
        private double[][] _deltas;
        private double[][] _previousBiasGradientEME;
        private double[][][] _previousWeightGradientEME;

        public double[][][] Weights { get; private set; }
        public double[][] Biases { get; private set; }

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

            Biases = new double[_layerNumber][];
            Weights = new double[_layerNumber][][];

            _deltas = new double[_layerNumber][];
            _layerOutputs = new double[_layerNumber][];
            _layerInputs = new double[_layerNumber][];
            _previousBiasGradientEME = new double[_layerNumber][];
            _previousWeightGradientEME = new double[_layerNumber][][];
            for (int l = 0; l < _layerNumber; l++)
            {
                Biases[l] = new double[_nonInputLayerSizes[l]];
                Weights[l] = new double[GetInputsNumberForLayer(l)][];

                _deltas[l] = new double[_nonInputLayerSizes[l]];
                _layerOutputs[l] = new double[_nonInputLayerSizes[l]];
                _layerInputs[l] = new double[_nonInputLayerSizes[l]];
                _previousBiasGradientEME[l] = new double[_nonInputLayerSizes[l]];
                _previousWeightGradientEME[l] = new double[GetInputsNumberForLayer(l)][];
                for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                {
                    Weights[l][i] = new double[_nonInputLayerSizes[l]];
                    _previousWeightGradientEME[l][i] = new double[_nonInputLayerSizes[l]];
                }
            }
        }

        public void Set(double[][][] weights, double[][] biases)
        {
            Weights = weights;
            Biases = biases;
        }

        public void SetRandom()
        {
            var random = new Random();
            for (int l = 0; l < _layerNumber; l++)
            {
                for (int j = 0; j < _nonInputLayerSizes[l]; j++)
                {
                    Biases[l][j] = random.NextDoubleBetween(-1, 1);
                }

                for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                {
                    for (int j = 0; j < _nonInputLayerSizes[l]; j++)
                    {
                        Weights[l][i][j] = random.NextDoubleBetween(-1, 1);
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
                    _layerInputs[l][j] = Biases[l][j];
                    for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                    {
                        double input = GetInputFromLayer(inputs, l, i);
                        _layerInputs[l][j] += Weights[l][i][j] * input;
                    }
                    _layerOutputs[l][j] = _activationFunctions[l].Evaluate(_layerInputs[l][j]);
                }
            }
            return (double[])_layerOutputs[_layerNumber - 1].Clone();
        }

        public double Train(double[] inputs, double[] targets)
        {
            double error = BackPropagateError(inputs, targets);
            UpdateWeights(inputs);
            UpdateBiases();
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
                    _deltas[layerIndex][i] += Weights[layerIndex + 1][i][j] * _deltas[layerIndex + 1][j];
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

        private void UpdateWeights(double[] inputs)
        {
            for (int l = 0; l < _layerNumber; l++)
            {
                for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                {
                    for (int j = 0; j < _nonInputLayerSizes[l]; j++)
                    {
                        double gradient = _deltas[l][j] * GetInputFromLayer(inputs, l, i);
                        _previousWeightGradientEME[l][i][j] = _beta * _previousWeightGradientEME[l][i][j] +
                            (1 - _beta) * gradient * gradient;
                        Weights[l][i][j] -= _learningRate * gradient / Math.Sqrt(_epsilon + _previousWeightGradientEME[l][i][j]);
                    }
                }
            }
        }

        private void UpdateBiases()
        {
            for (int l = 0; l < _layerNumber; l++)
            {
                for (int i = 0; i < _nonInputLayerSizes[l]; i++)
                {
                    double gradient = _deltas[l][i];
                    _previousBiasGradientEME[l][i] = _beta * _previousBiasGradientEME[l][i] +
                        (1 - _beta) * gradient * gradient;
                    Biases[l][i] -= _learningRate * gradient / Math.Sqrt(_epsilon + _previousBiasGradientEME[l][i]);
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