using WebApi.Core;

namespace WebApi.Persistence.ActivationFunctions
{
    public class ReLU : IActivationFunction
    {
        public double Evaluate(double value)
        {
            return (value > 0) ? value : 0;
        }

        public double EvaluateDerivative(double value)
        {
            return (value > 0) ? 1 : 0;
        }
    }
}