using PingaUnitBooking.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PingaUnitBooking.Infrastructure.Interfaces
{
    public interface IProjectInterface
    {
        Task<ResponseDataResults<List<ProjectAllData>>> searchData(SearchData _searchData);
        Task<ResponseDataResults<int>> addProjectPermission(BindProjectPermissionDTO _bindProjectPermissionDTO);

        Task<ResponseDataResults<List<BindProjectPermissionDTO>>> projectPermissionlist(decimal? groupID, int? userID);
        Task<ResponseDataResults<int>> deletePermission(int? permissionID, decimal? groupID);
    }
}
