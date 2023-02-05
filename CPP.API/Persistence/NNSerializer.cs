using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CPP.API.Core;

namespace CPP.API.Persistence
{
    public class NNSerializer : INNSerializer
    {
        public void Serialize(string path, INN nn)
        {
            using var stream = File.Open(path, FileMode.Create);
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, nn);
        }

        public INN Deserialize(string path)
        {
            using var stream = File.Open(path, FileMode.Open);
            var binaryFormatter = new BinaryFormatter();
            return (INN)binaryFormatter.Deserialize(stream);
        }
    }
}