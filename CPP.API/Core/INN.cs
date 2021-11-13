namespace CPP.API.Core
{
    public interface INN
    {
        double[][] Biases { get; }
        double[][][] Weights { get; }
        double[] Run(double[] input);
        double Train(double[] inputs, double[] targets);
        void Initialize(int[] layerSizes, IActivationFunction[] activationFunctions);
        void Set(double[][][] weights, double[][] biases);
        void SetRandom();
    }
}