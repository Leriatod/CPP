namespace CPP.API.Core.Models
{
    public class Car
    {
        public string Producer { get; set; }
        public string Model { get; set; }
        public string Body { get; set; }
        public string Drive { get; set; }
        public string Transmission { get; set; }
        public string Fuel { get; set; }
        public double Engine { get; set; }
        public double Horsepower { get; set; }
        public double Distance { get; set; }
        // Double because we need to feed it to our NN
        public double Year { get; set; }
        public double Price { get; set; }
    }
}