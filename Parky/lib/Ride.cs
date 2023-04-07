using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parky.lib
{
    public class Ride
    {
        public Ride() { }
        public Ride(Ride ride) { }
        public int id { get; set; }
        public string name { get; set; }
        public bool is_open { get; set; }
        public string wait_time { get; set; }
        public string last_updated { get; set; }
        public string inLand { get; set; }
        public Color waitCol { get; set; }
        public int wait_time_true { get; set; }
    }
}
