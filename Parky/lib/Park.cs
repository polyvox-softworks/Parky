using Newtonsoft.Json;
using Parky.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parky.lib
{
    [Serializable]
    public class Park
    {
        public Park() { }

        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("country")]
        public string country { get; set; }
        [JsonProperty("continent")]
        public string continent { get; set; }
        [JsonProperty("latitude")]
        public string latitude { get; set; }
        [JsonProperty("longitude")]
        public string longitude { get; set; }
        [JsonProperty("timezone")]
        public string timezone { get; set; }

        public string img { get; set; }

        [JsonProperty("lands")]
        public List<Land> lands { get; set; }

        [JsonProperty("rides")]
        public List<Ride> ridesNoLands { get; set; }

        public string company { get; set; }

        public Point location { get; set; }
        public double distanceFromCurrentLocation { get; set; }

        public Day schedule {  get; set; }

    }
}
