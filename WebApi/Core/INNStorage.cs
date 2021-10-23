using WebApi.Core.Models;

namespace WebApi.Core
{
    public interface INNStorage
    {
        void Save(NNCoefficients nnCoefficients);
        NNCoefficients Load();
    }
}