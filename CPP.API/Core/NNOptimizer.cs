using System;

namespace CPP.API.Core
{
    [Serializable]
    public abstract class NNOptimizer
    {
        protected readonly double _learningRate;
        protected readonly double _clipValue;

        public NNOptimizer(double learningRate = 0.001, double clipValue = 0.0)
        {
            _clipValue = clipValue;
            _learningRate = learningRate;
        }

        // to avoid exploding gradient problem, if _clipValue is 0 - no gradient clipping
        protected double ClipGradient(double gradient)
        {
            if (_clipValue == 0.0) return gradient;
            if (gradient > _clipValue) return _clipValue;
            if (gradient < -_clipValue) return -_clipValue;
            return gradient;
        }

        public abstract void Initialize(double[][][] coefficients);
        public abstract void UpdateCoefficient(int layerIndex, int neuronIndex, int inputIndex, double gradient);
    }
}