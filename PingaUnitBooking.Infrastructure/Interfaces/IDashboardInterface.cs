using PingaUnitBooking.Core.Domain;


namespace PingaUnitBooking.Infrastructure.Interfaces
{
    public interface IDashboardInterface
    {
        Task<ResponseDataResults<Dashboard>> GetDashboardSummary(decimal GroupID,int UserID,string YearMonth);
        Task<ResponseDataResults<string>> GetUbmEmails(decimal GroupID, int UbmID);

    }
}
