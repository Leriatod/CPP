namespace CPP.API.Core
{
    public interface INNOptimizer
    {
        void Initialize(double[][][] weights, double[][] biases);
        void UpdateWeight(int layerIndex, int neuronIndex, int inputIndex, double gradient);
        void UpdateBias(int layerIndex, int neuronIndex, double gradient);
    }
}