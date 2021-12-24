namespace CPP.API.Core
{
    public interface INNOptimizer
    {
        void UpdateWeight(double[][][] weights, int layerIndex, int inputIndex, int neuronIndex, double gradient);
        void UpdateBias(double[][] biases, int layerIndex, int neuronIndex, double gradient);
    }
}