
namespace CPP.API.Core
{
    public interface INN
    {
        double[] Run(double[] input);
        double Train(double[] inputs, double[] targets);
        void Initialize(int[] layerSizes, IActivationFunction[] activationFunctions, INNOptimizer optimizer);
        void SetRandomCoefficients();
    }
}