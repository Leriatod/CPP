namespace CPP.API.Core
{
    public interface INNSerializer
    {
        void Serialize(string path, INN nn);
        INN Deserialize(string path);
    }
}