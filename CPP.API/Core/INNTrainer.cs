namespace CPP.API.Core
{
    public interface INNTrainer
    {
        void Train(int epochNumber, string nnReadPath, string nnWritePath);
    }
}