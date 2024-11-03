namespace PingaUnitBooking.Core.Domain
{
    public class CommunicationChannel
    {
        public int? ChannelID { get; set; }
        public decimal GroupID { get; set; }
        public bool IsWhatsApp { get; set; }
        public bool IsEmail { get; set; }
        public bool IsSms { get; set; }
        public int? CreatedBy { get; set; }
    }
}
