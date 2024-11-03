using PingaUnitBooking.Core.Domain;


namespace PingaUnitBooking.Infrastructure.Interfaces
{
    public interface IMailConfigureInterface
    {
        Task<ResponseDataResults<int>> SaveMailConfigure(MailConfigure mailConfigure);
        Task<ResponseDataResults<List<MailConfigure>>> GetMailConfigure(decimal GroupID);
        Task<ResponseDataResults<List<Template>>> GetNotificationTemplate(decimal GroupID,int ProjectID ,string ProcessType);
        Task<ResponseDataResults<Communication>> GetCustomerUnitDetail(int BookingID);
        Task<ResponseDataResults<int>> DeleteMailConfigure(int MailConfigureID);
        Task<ResponseDataResults<int>> SaveMailHistory(MailHistory mailHistory);

    }
}
