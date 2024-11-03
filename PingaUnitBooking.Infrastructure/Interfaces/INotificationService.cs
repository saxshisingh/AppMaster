using PingaUnitBooking.Core.Domain;

namespace PingaUnitBooking.Infrastructure.Interfaces
{
    public  interface INotificationService
    {
        Task SendNotifiction(decimal GroupID, int UserID, int BookingID, string ProcessType);
        
        Task<ResponseDataResults<string>> SendReallocationMail(TestMail testMail);
        Task<ResponseDataResults<string>> TestMailConfigure(TestMail testMail);
        Task<ResponseDataResults<string>> AlertMail(TestMail testMail);
    }
}
