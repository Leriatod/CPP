using CPP.Core.Models;
using CsvHelper.Configuration;

namespace CPP.Mapping
{
    public class CarClassMap : ClassMap<Car>
    {
        public CarClassMap()
        {
            Map(c => c.Name).Name("name");
            Map(c => c.Model).Name("model");
            Map(c => c.BodyStyle).Name("type");
            Map(c => c.Price).Name("price");
            Map(c => c.Year).Name("year");
            Map(c => c.KilometersDriven).Name("distance");
            Map(c => c.TransmissionType).Name("transmission");
            Map(c => c.EngineCapacity).Name("engine");
            Map(c => c.Color).Name("color");
            Map(c => c.IsActive).Name("isActive");
        }
    }
}