using PingaUnitBooking.Core.Domain; 

namespace PingaUnitBooking.Infrastructure.Interfaces
{
    public interface IReallocationInterface
    {
        Task<ResponseDataResults<List<KeyValuePair<int, string>>>> GetUserByRoleName(decimal GroupID,string RoleName);
        Task<ResponseDataResults<List<Reallocation>>> GetReallocationUnit(decimal GroupID, string RoleName,int UserID);
        Task<ResponseDataResults<int>> SaveBookingReallocation(decimal GroupID, int FromUserID,int ToUserID,  string BookingIDs, int CreatedBy);

    }
}
