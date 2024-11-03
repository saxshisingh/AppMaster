using PingaUnitBooking.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PingaUnitBooking.Infrastructure.Interfaces
{
   public interface IAuthInterface
    {
        Task<ResponseDataResults<AuthData>> userLogin(AuthData auth);
        Task<ResponseDataResults<List<AuthData>>> customerAuth(AuthData auth);
        Task<ResponseDataResults<int>> addUser(ubmUserData _userData);
        Task<ResponseDataResults<int>> changeStatus(int? userID , decimal? groupID);

        Task<ResponseDataResults<List<AuthData>>> userList(decimal? groupID, int? roleID , string? type) ;
        Task<ResponseDataResults<int>> updateToken(decimal? userID, string? token, decimal? groupID);
        Task<ResponseDataResults<List<RoleMaster>>> GetPermissions(int? userID, decimal? groupID, string? pageType);
    }
}
