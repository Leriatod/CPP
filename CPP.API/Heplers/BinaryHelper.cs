using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CPP.API.Heplers
{
    public static class BinaryHelper
    {
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite)
        {
            using var stream = File.Open(filePath, FileMode.Create);
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
        }

        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using var stream = File.Open(filePath, FileMode.Open);
            var binaryFormatter = new BinaryFormatter();
            return (T)binaryFormatter.Deserialize(stream);
        }
    }
}