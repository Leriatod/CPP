using CPP.API.Core;

namespace CPP.API.Persistence.ActivationFunctions
{
    public class Linear : IActivationFunction
    {
        public double Evaluate(double value)
        {
            return value;
        }

        public double EvaluateDerivative(double value)
        {
            return 1;
        }
    }
}