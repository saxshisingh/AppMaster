using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
    public class BookingData
    {
        public int? ubmID { get; set; }
        public string? UnitType { get; set; }
        public int? UnitID { get; set; }
        public string? CustomerName { get; set; }
        public decimal? CustomerMobileNo { get; set; }
        public string? CustomerEmail { get; set; }
        public DateTime? releaseUnitDate { get; set; }
        public string? applicationType { get; set; }
        public int createdBy { get; set; }
        public string? UnitNo { get; set; }
        public string? ProjectName { get; set; }
        public decimal? ProjectID { get; set; }
        public decimal? TowerID { get; set; }
        public string? TowerName { get; set; }
        public decimal? FloorID { get; set; }
        public string? FloorName { get; set; }
        public int? Status { get; set; }
        public decimal groupID { get; set; }
        public int? IsVisible { get; set; }
        public string? Remarks { get; set; }
        public string? BrokerName { get; set; }
        public int? BrokerID { get; set; }
        public decimal? BrokerDiscount { get; set; }
        public decimal? NetAmount { get; set; }

    }

    public class UnitLogs
    {
        public string StatusName { get; set; }
        public string Remarks { get; set; }
        public string CreatedAt { get; set; }
        public string UserName { get; set; }
    }

    public class BrokerData
    {
        public int BrokerID { get; set; }
        public string BrokerName { get; set; }
        public decimal GroupID { get; set; }
    }
}
