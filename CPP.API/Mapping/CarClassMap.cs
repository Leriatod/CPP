using CPP.API.Core.Models;
using CsvHelper.Configuration;

namespace CPP.API.Mapping
{
    public class CarClassMap : ClassMap<Car>
    {
        public CarClassMap()
        {
            Map(c => c.Manufacturer).Name("manufacturer");
            Map(c => c.Model).Name("model");
            Map(c => c.Body).Name("body_type");
            Map(c => c.Drive).Name("drive_type");
            Map(c => c.Transmission).Name("transmission_type");
            Map(c => c.EngineCapacity).Name("engine_capacity");
            Map(c => c.Horsepower).Name("horsepower");
            Map(c => c.Fuel).Name("fuel_type");
            Map(c => c.Mileage).Name("mileage");
            Map(c => c.ManufactureYear).Name("manufacture_year");
            Map(c => c.Price).Name("price");
        }
    }
}