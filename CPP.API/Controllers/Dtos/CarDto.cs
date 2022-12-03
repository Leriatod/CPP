namespace CPP.API.Controllers.Dtos
{
    public class CarDto
    {
        public string Model { get; set; }
        public string Body { get; set; }
        public string Drive { get; set; }
        public string Transmission { get; set; }
        public string Fuel { get; set; }
        public double EngineCapacity { get; set; }
        public double Horsepower { get; set; }
        public double Mileage { get; set; }
        public int ManufactureYear { get; set; }
    }
}