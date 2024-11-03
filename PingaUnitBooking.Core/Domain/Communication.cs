 

namespace PingaUnitBooking.Core.Domain
{
    public class Communication
    {
        public int BookingID { get; set; } 
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public DateTime? BookingDate { get; set; }
        public string BookingUrl { get; set; }
        public string ApplicationType { get; set; }
        public decimal BookingAmount { get; set; }
        public string SalesPerson { get; set; } 
        public string Password { get; set; }
        public int PayPlanID { get; set; }
        public UnitData UnitDetail { get; set; }
    }
}
