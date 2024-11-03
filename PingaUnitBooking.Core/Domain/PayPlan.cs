using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
    public class PayPlan
    {
        public int StageID { get; set; }
        public string StageName { get; set; }
        public string ChargeName { get; set; }
        public decimal DuePercentage { get; set; }
    }
}
