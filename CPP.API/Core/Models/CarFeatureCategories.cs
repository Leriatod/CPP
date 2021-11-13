using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace CPP.API.Core.Models
{
    public class CarFeatureCategories
    {
        public IEnumerable<string> Producers { get; set; }
        public IEnumerable<string> Models { get; set; }
        public IEnumerable<string> Bodies { get; set; }
        public IEnumerable<string> Drives { get; set; }
        public IEnumerable<string> Transmissions { get; set; }
        public IEnumerable<string> Fuels { get; set; }
    }
}