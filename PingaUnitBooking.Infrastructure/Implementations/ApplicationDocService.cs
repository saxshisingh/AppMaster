using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Infrastructure.Implementations
{
    public class ApplicationDocService : IApplicationDocInterface
    {
        private readonly IDbInterface _dbInterface;
        public ApplicationDocService(IDbInterface _dbInterface)
        {
            this._dbInterface = _dbInterface;
        }
        public async Task<ResponseDataResults<int>> SaveApplicationDocument(ApplicationDoc _ubmappdoc)
        {
            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_SaveAppDoc", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DocumentID", _ubmappdoc.DocumentID);
                        command.Parameters.AddWithValue("@GroupID", _ubmappdoc.GroupID);
                        command.Parameters.AddWithValue("@ApplicationType", _ubmappdoc.ApplicationType);
                        command.Parameters.AddWithValue("@DocumentName", _ubmappdoc.DocumentName);
                        command.Parameters.AddWithValue("@CreatedBy", _ubmappdoc.UserID);
                        command.Parameters.AddWithValue("@IsMandatory", _ubmappdoc.Mandatory);
                        i = await command.ExecuteNonQueryAsync();
                    }
                    return new ResponseDataResults<int>
                    {
                        IsSuccess = true,
                        Message = "Data Save Successfully..",
                        Data = i
                    };
                }
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

        public async Task<ResponseDataResults<List<ApplicationDoc>>> GetApplicationDocList(decimal GroupID)
        {
            try
            {
                List<ApplicationDoc> appdocList = new List<ApplicationDoc>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_GetAppDocList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@GroupID", GroupID));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                ApplicationDoc appdoc = new ApplicationDoc();
                                appdoc.DocumentID = reader.GetInt32(reader.GetOrdinal("DocumentID"));
                                appdoc.ApplicationType = reader.GetString(reader.GetOrdinal("ApplicationType"));
                                appdoc.DocumentName = reader.GetString(reader.GetOrdinal("DocumentName"));
                                appdoc.Mandatory = reader.GetBoolean(reader.GetOrdinal("IsMandatory"));
                                appdocList.Add(appdoc);
                            }
                        }
                    }
                    return new ResponseDataResults<List<ApplicationDoc>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = appdocList
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<ApplicationDoc>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<ApplicationDoc>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDataResults<int>> DeleteApplicationDocument(int id)
        {
            int IsDelete = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_DeleteAppDoc", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DocumentID", id);
                        IsDelete= await command.ExecuteNonQueryAsync();
                    }
                    return new ResponseDataResults<int>
                    {
                        IsSuccess = true,
                        Message = "Data Deleted Successfully..",
                        Data = IsDelete
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<int>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = IsDelete
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<int>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = IsDelete
                };
            }
        }
    }
}
