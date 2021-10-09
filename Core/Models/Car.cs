namespace CPP.Core.Models
{
    public class Car
    {
        public string Producer { get; set; }
        public string Model { get; set; }
        public string BodyStyle { get; set; }
        public double Price { get; set; }
        public int Year { get; set; }
        public double KilometersDriven { get; set; }
        public string TransmissionType { get; set; }
        // FIXME: shouldn't be string, we should change our data format so this feature is number 
        public string EngineCapacity { get; set; }
        public string Color { get; set; }
        public bool IsActive { get; set; }
    }
}