using System;
using CPP.API.Core;

namespace CPP.API.Persistence.ActivationFunctions
{
    public class Tanh : IActivationFunction
    {
        public double Evaluate(double value)
        {
            return 2 / (1 + Math.Exp(-2 * value)) - 1;
        }

        public double EvaluateDerivative(double value)
        {
            return 1 - Math.Pow(Evaluate(value), 2);
        }
    }
}