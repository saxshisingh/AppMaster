using PingaUnitBooking.Core.Domain;


namespace PingaUnitBooking.Infrastructure.Interfaces
{
    public interface ISchemeInterface
    {
        Task<ResponseDataResults<int>> SaveScheme(Scheme _scheme);
        Task<ResponseDataResults<List<Scheme>>> GetSchemeList(decimal GroupID);
        Task<ResponseDataResults<int>> DeleteScheme(int SchemeId);
    }
}
