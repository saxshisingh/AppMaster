using PingaUnitBooking.Core.Domain;


namespace PingaUnitBooking.Infrastructure.Interfaces
{
    public interface IApplicationDocInterface
    {
        Task<ResponseDataResults<int>> SaveApplicationDocument(ApplicationDoc _ubmappdoc);

        Task<ResponseDataResults<List<ApplicationDoc>>> GetApplicationDocList(decimal GroupID);

        Task<ResponseDataResults<int>> DeleteApplicationDocument(int id);

        
    }
}
