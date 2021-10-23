namespace CPP.Core.Models
{
    public class NNCoefficients
    {
        public double[][] Biases { get; set; }
        public double[][][] Weights { get; set; }
        public NNCoefficients(double[][] biases, double[][][] weights)
        {
            Biases = biases;
            Weights = weights;
        }
    }
}