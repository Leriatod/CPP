using System.IO;
using System.Text.Json;
using CPP.Core;
using CPP.Core.Models;

namespace CPP.Persistence
{
    public class NNStorage : INNStorage
    {
        private readonly string filePath = "Persistence/model.json";
        public void Save(NNCoefficients nnCoefficients)
        {
            string json = JsonSerializer.Serialize(nnCoefficients);
            File.WriteAllText(filePath, json);
        }
    }
}