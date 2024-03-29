using CPP.API.Core.Models;
using CsvHelper.Configuration;

namespace CPP.API.Mapping
{
    public class CarCsvMapper : ClassMap<Car>
    {
        public CarCsvMapper()
        {
            Map(c => c.Model).Convert(args => $"{args.Row.GetField("manufacturer")} {args.Row.GetField("model")}");
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