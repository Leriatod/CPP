using CPP.API.Core.Models;
using CsvHelper.Configuration;

namespace CPP.API.Mapping
{
    public class CarClassMap : ClassMap<Car>
    {
        public CarClassMap()
        {
            Map(c => c.Producer).Name("Producer");
            Map(c => c.Model).Name("Model");
            Map(c => c.Body).Name("Body");
            Map(c => c.Drive).Name("Drive");
            Map(c => c.Transmission).Name("Transmission");
            Map(c => c.Engine).Name("Engine");
            Map(c => c.Horsepower).Name("Horsepower");
            Map(c => c.Fuel).Name("Fuel");
            Map(c => c.Distance).Name("Distance");
            Map(c => c.Year).Name("Year");
            Map(c => c.Price).Name("Price");
        }
    }
}