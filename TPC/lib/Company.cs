using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPC.lib
{
    public class Company
    {
        public Company() { }
        public int id { get; set; }
        public string name { get; set; }
        public List<Park> parks { get; set; }
    }
}
