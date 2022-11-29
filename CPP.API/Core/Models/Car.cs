namespace CPP.API.Core.Models
{
    public class Car
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Body { get; set; }
        public string Drive { get; set; }
        public string Transmission { get; set; }
        public string Fuel { get; set; }
        public double EngineCapacity { get; set; }
        public double Horsepower { get; set; }
        public double Mileage { get; set; }
        // Double because we need to feed it to our NN
        public double ManufactureYear { get; set; }
        public double Price { get; set; }
    }
}