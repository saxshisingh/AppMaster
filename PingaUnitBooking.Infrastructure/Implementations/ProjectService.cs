using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace PingaUnitBooking.Infrastructure.Implementations
{
    public class ProjectService : IProjectInterface
    {
        private readonly IDbInterface _dbInterface;
        public ProjectService(IDbInterface _dbInterface )
        {
            this._dbInterface = _dbInterface;
        }
        public async Task<ResponseDataResults<List<ProjectAllData>>> searchData(SearchData _searchData)
        {
            try
            {
                List<ProjectAllData> _projectAllData = new List<ProjectAllData>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_getProjectData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@search", _searchData.search);
                        command.Parameters.AddWithValue("@type", _searchData.type);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                ProjectAllData projectData = new ProjectAllData();

                                if (_searchData.type == "Company")
                                {
                                    mstCompany company = new mstCompany
                                    {
                                        companyID = reader.GetDecimal(reader.GetOrdinal("companyID")),
                                        companyName = reader.GetString(reader.GetOrdinal("companyName")),
                                    };
                                    projectData.company = new List<mstCompany> { company };
                                }
                                else if (_searchData.type == "Location")
                                {
                                    mstLocation location = new mstLocation
                                    {
                                        locationID = reader.GetDecimal(reader.GetOrdinal("LocationID")),
                                        locationName = reader.GetString(reader.GetOrdinal("LocationName")),
                                    };
                                    projectData.location = new List<mstLocation> { location };
                                }
                                else if (_searchData.type == "Project")
                                {
                                    mstProject project = new mstProject
                                    {
                                        projectID = reader.GetDecimal(reader.GetOrdinal("ProjectID")),
                                        projectName = reader.GetString(reader.GetOrdinal("ProjectName")),
                                    };
                                    projectData.project = new List<mstProject> { project };
                                }
                                else if (_searchData.type == "Tower")
                                {
                                    mstTower tower = new mstTower
                                    {
                                        towerID = reader.GetDecimal(reader.GetOrdinal("TowerID")),
                                        towerName = reader.GetString(reader.GetOrdinal("TowerName")),
                                    };
                                    projectData.tower = new List<mstTower> { tower };
                                }
                                _projectAllData.Add(projectData);
                            }
                        }
                    }
                }

                return new ResponseDataResults<List<ProjectAllData>>
                {
                    IsSuccess = true,
                    Message = "Data Fetch Successfully",
                    Data = _projectAllData
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<ProjectAllData>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<ProjectAllData>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDataResults<int>> addProjectPermission(BindProjectPermissionDTO _bindProjectPermissionDTO)
        {
            int i = 0;
            try
            {

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_AddUpdateProjectPermission", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ubmUserID", _bindProjectPermissionDTO.userID);
                        command.Parameters.AddWithValue("@projectID", _bindProjectPermissionDTO.projectid);
                        command.Parameters.AddWithValue("@towerID", _bindProjectPermissionDTO.towerID);
                        command.Parameters.AddWithValue("@CreatedBy", _bindProjectPermissionDTO.CreatedBy);
                        command.Parameters.AddWithValue("@GroupID", _bindProjectPermissionDTO.GroupID);
                       i= await command.ExecuteNonQueryAsync();
                    }
                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Successfully Added Permission",
                    Data = i
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<int>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = i
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<int>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = i
                };
            }
        }


        public async Task<ResponseDataResults<List<BindProjectPermissionDTO>>> projectPermissionlist(decimal? groupID, int? userID)
        {
            try
            {
                List<BindProjectPermissionDTO> _bindProjectPermissionDTO = new List<BindProjectPermissionDTO>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_getProjectListByUserID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@groupID", groupID);
                        command.Parameters.AddWithValue("@userID", userID);
                       
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                BindProjectPermissionDTO _bpp = new BindProjectPermissionDTO();

                                    _bpp.ProjectPermissionID = reader.GetInt32(reader.GetOrdinal("ProjectPermissionID"));
                                    _bpp.companyName = reader.GetString(reader.GetOrdinal("CompanyName"));
                                    _bpp.locationName = reader.GetString(reader.GetOrdinal("LocationName"));
                                    _bpp.projectName = reader.GetString(reader.GetOrdinal("ProjectName"));
                                    _bpp.towerName = reader.GetString(reader.GetOrdinal("TowerName"));

                                _bindProjectPermissionDTO.Add(_bpp);
                            }
                        }
                    }
                    return new ResponseDataResults<List<BindProjectPermissionDTO>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = _bindProjectPermissionDTO
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<BindProjectPermissionDTO>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<BindProjectPermissionDTO>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }


        public async Task<ResponseDataResults<int>> deletePermission(int? permissionID, decimal? groupID)
        {
            int i = 0;
            try
            {

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_deleteProjectPermission", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@permissionID", permissionID);
                        command.Parameters.AddWithValue("@groupID", groupID);

                       i= await command.ExecuteNonQueryAsync();
                    }
                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Successfully Delete the Project Permission",
                    Data = i
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<int>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = i
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<int>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = i
                };
            }
        }



    }
}
