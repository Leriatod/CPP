using System;
using CPP.API.Core;
using CPP.API.Extensions;

namespace CPP.API.Persistence
{
    [Serializable]
    public class NN : INN
    {
        private IActivationFunction[] _activationFunctions;
        private NNOptimizer _optimizer;
        private double _l2Lambda;
        private double[][][] _coefficients;
        private double[][] _layerOutputs;
        private double[][] _layerInputs;
        private double[][] _deltas;
        private double[] _inputs;
        private int LayerNumber => _coefficients.Length;
        private int LastLayerIndex => LayerNumber - 1;

        public void Initialize(int[] layerSizes, IActivationFunction[] activationFunctions, NNOptimizer optimizer, double l2Lambda = 0.001)
        {
            _optimizer = optimizer;
            _activationFunctions = activationFunctions;
            _l2Lambda = l2Lambda;

            _coefficients = new double[layerSizes.Length - 1][][];
            _deltas = new double[LayerNumber][];
            _layerOutputs = new double[LayerNumber][];
            _layerInputs = new double[LayerNumber][];

            for (int l = 0; l < LayerNumber; l++)
            {
                _coefficients[l] = new double[layerSizes[l + 1]][];
                _deltas[l] = new double[GetLayerSize(l)];
                _layerOutputs[l] = new double[GetLayerSize(l)];
                _layerInputs[l] = new double[GetLayerSize(l)];

                for (int j = 0; j < GetLayerSize(l); j++)
                {
                    _coefficients[l][j] = new double[layerSizes[l] + 1];
                }
            }

            _optimizer.Initialize(_coefficients);
        }

        public void SetRandomCoefficients()
        {
            var random = new Random();
            for (int l = 0; l < LayerNumber; l++)
            {
                for (int j = 0; j < GetLayerSize(l); j++)
                {
                    for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                    {
                        _coefficients[l][j][i] = random.NextDoubleBetween(-1, 1);
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
                    _layerInputs[l][j] = 0.0;
                    for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                    {
                        _layerInputs[l][j] += _coefficients[l][j][i] * GetInput(l, i);
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
            int nextLayerIndex = layerIndex + 1;
            for (int i = 0; i < GetLayerSize(layerIndex); i++)
            {
                _deltas[layerIndex][i] = 0.0;
                for (int j = 0; j < GetLayerSize(nextLayerIndex); j++)
                {
                    _deltas[layerIndex][i] += _coefficients[nextLayerIndex][j][i + 1] * _deltas[nextLayerIndex][j];
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
                    for (int i = 0; i < GetInputsNumberForLayer(l); i++)
                    {
                        _optimizer.UpdateCoefficient(l, j, i,
                            gradient: _deltas[l][j] * GetInput(l, i) + _l2Lambda * _coefficients[l][j][i]);
                    }
                }
            }
        }

        private double GetInput(int layerIndex, int inputIndex)
        {
            if (inputIndex == 0) return 1;
            if (layerIndex == 0) return _inputs[inputIndex - 1];
            return _layerOutputs[layerIndex - 1][inputIndex - 1];
        }

        private int GetLayerSize(int layerIndex) => _coefficients[layerIndex].Length;
        private int GetInputsNumberForLayer(int layerIndex) => _coefficients[layerIndex][0].Length;
    }
}