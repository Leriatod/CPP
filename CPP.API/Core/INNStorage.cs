using CPP.API.Core.Models;

namespace CPP.API.Core
{
    public interface INNStorage
    {
        void Save(NNCoefficients nnCoefficients);
        NNCoefficients Load();
    }
}