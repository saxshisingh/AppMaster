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
    public class SchemeService : ISchemeInterface
    {
        private readonly IDbInterface _dbInterface;
        public SchemeService(IDbInterface _dbInterface)
        {
            this._dbInterface = _dbInterface;
        }
        public async Task<ResponseDataResults<int>> SaveScheme(Scheme _scheme)
        {
            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_SaveScheme", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SchemeID", _scheme.SchemeID);
                        command.Parameters.AddWithValue("@GroupID", _scheme.GroupID);
                        command.Parameters.AddWithValue("@SchemeName", _scheme.SchemeName);
                        command.Parameters.AddWithValue("@SchemeDesc", _scheme.SchemeDesc);
                        command.Parameters.AddWithValue("@CreatedBy", _scheme.CreatedBy); 
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

        public async Task<ResponseDataResults<List<Scheme>>> GetSchemeList(decimal GroupID)
        {
            List<Scheme> schemeList = new List<Scheme>();
            try
            { 
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_GetSchemeList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@GroupID", GroupID));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                Scheme scheme = new Scheme();
                                scheme.SchemeID = Convert.ToInt32(reader["SchemeID"]);
                                scheme.SchemeName = Convert.ToString(reader["SchemeName"]);
                                scheme.SchemeDesc = Convert.ToString(reader["SchemeDesc"]); 
                                schemeList.Add(scheme);
                            }
                        }
                    }
                    return new ResponseDataResults<List<Scheme>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = schemeList
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<Scheme>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = schemeList
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<Scheme>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = schemeList
                };
            }
        }

        public async Task<ResponseDataResults<int>> DeleteScheme(int SchemeId)
        {
            int IsDelete = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_DeleteScheme", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SchemeID", SchemeId);
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
