using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parky.lib
{
    public class Land
    {
        public Land() { }
        public int id { get; set; }
        public string name { get; set; }

        [JsonProperty("rides")]
        public List<Ride> rides { get; set; }
    }
}
