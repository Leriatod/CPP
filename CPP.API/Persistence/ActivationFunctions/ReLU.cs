using System;
using CPP.API.Core;

namespace CPP.API.Persistence.ActivationFunctions
{
    [Serializable]
    public class ReLU : IActivationFunction
    {
        public double Evaluate(double value) => (value > 0) ? value : 0;
        public double EvaluateDerivative(double value) => (value > 0) ? 1 : 0;
    }
}