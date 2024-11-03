using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
    public class States
    {
        public int StateID { get; set; }
        public string StateName { get; set;}
    }
    public class Cities
    {
        public int CityID { get;  set; }
        public int StateID { get; set; }
        public string CityName { get; set; }
    }
}
