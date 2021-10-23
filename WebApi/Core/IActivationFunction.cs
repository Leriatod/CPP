namespace WebApi.Core
{
    public interface IActivationFunction
    {
        double Evaluate(double value);
        double EvaluateDerivative(double value);
    }
}