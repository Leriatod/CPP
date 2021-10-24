using System.IO;
using System.Text.Json;
using CPP.API.Core;
using CPP.API.Core.Models;

namespace CPP.API.Persistence
{
    public class NNStorage : INNStorage
    {
        private readonly string _filePath = "Persistence/model.json";
        private readonly JsonSerializerOptions _serializerOptions = new()
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