using CPP.Core.Models;

namespace CPP.Core
{
    public interface INNStorage
    {
        void Save(NNCoefficients nnCoefficients);
    }
}