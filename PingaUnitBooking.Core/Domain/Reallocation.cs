namespace PingaUnitBooking.Core.Domain
{
    public class Reallocation
    {
        public int BookingID { get; set; } 
        public string ProjectName { get; set; }
        public string TowerName { get; set; }
        public string FloorName { get; set; }
        public string UnitNo { get; set; }
        public string StatusName { get; set; }
        public int CreatedBy { get; set; }
    }
}
