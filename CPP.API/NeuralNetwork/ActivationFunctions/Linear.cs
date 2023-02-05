using System;
using CPP.API.Core;

namespace CPP.API.NeuralNetwork.ActivationFunctions
{
    [Serializable]
    public class Linear : IActivationFunction
    {
        public double Evaluate(double value) => value;
        public double EvaluateDerivative(double value) => 1;
    }
}