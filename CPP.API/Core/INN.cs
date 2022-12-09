
namespace CPP.API.Core
{
    public interface INN
    {
        double[] Run(double[] input);
        double Train(double[] inputs, double[] targets);
        void Initialize(int[] layerSizes, IActivationFunction[] activationFunctions, NNOptimizer optimizer, double l2Lambda);
        void SetRandomCoefficients();
    }
}