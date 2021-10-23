using System.IO;
using System.Text.Json;
using CPP.Core;
using CPP.Core.Models;

namespace CPP.Persistence
{
    public class NNStorage : INNStorage
    {
        private readonly string _filePath = "Persistence/model.json";
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public NNCoefficients Load()
        {
            string json = File.ReadAllText(_filePath);
            var nnCoefficients = JsonSerializer.Deserialize<NNCoefficients>(json, _serializerOptions);
            return nnCoefficients;
        }

        public void Save(NNCoefficients nnCoefficients)
        {
            string json = JsonSerializer.Serialize(nnCoefficients, _serializerOptions);
            File.WriteAllText(_filePath, json);
        }
    }
}