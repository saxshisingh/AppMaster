namespace PingaUnitBooking.Core.Domain
{
    public class Scheme
    {
        public int SchemeID { get; set; }
        public decimal GroupID { get; set; }
        public string SchemeName { get; set; }
        public string SchemeDesc { get; set; }
        public int CreatedBy { get; set; }
    }
}
