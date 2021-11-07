namespace CPP.API.Core
{
    public interface INN
    {
        double[] Run(double[] input);
        double Train(double[] inputs, double[] targets, double learningRate, double momentumRate);
        void Initialize(int[] layerSizes, IActivationFunction[] activationFunctions);
        void Set(double[][][] weights, double[][] biases);
        void SetRandom();
    }
}