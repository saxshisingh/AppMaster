using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Channels;

namespace PingaUnitBooking.Infrastructure.Implementations
{
    public class TemplateService : ITemplateInterface
    {
        private readonly IDbInterface _dbInterface;
        public TemplateService(IDbInterface _dbInterface)
        {
            this._dbInterface = _dbInterface;
        }

        public async Task<ResponseDataResults<List<KeyValuePair<int, string>>>> ProcessTypeList()
        {

            ResponseDataResults<List<KeyValuePair<int, string>>> list = new ResponseDataResults<List<KeyValuePair<int, string>>>();
            list.Data = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_GetProcessType", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure; 
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {

                                int ID = Convert.ToInt32(Convert.ToString(reader["ID"]));
                                string ProcessType = Convert.ToString(reader["ProcessType"]);
                                list.Data.Add(new KeyValuePair<int, string>(ID, ProcessType));
                            }
                        }
                    }

                }
                return new ResponseDataResults<List<KeyValuePair<int, string>>>
                {
                    IsSuccess = true,
                    Message = "Data Reterival Successfully..",
                    Data = list.Data
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<KeyValuePair<int, string>>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = list.Data
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<KeyValuePair<int, string>>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = list.Data
                };
            }
        }
     

        public async Task<ResponseDataResults<string>> GetAppDocList(string ApplicationType, decimal GroupID)
        {
            string reader = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_GetAppDocs", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@GroupID", GroupID));
                        command.Parameters.Add(new SqlParameter("@ApplicationType", ApplicationType));
                        reader = Convert.ToString(await command.ExecuteScalarAsync());

                    }
                    return new ResponseDataResults<string>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = reader
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<string>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }
        public async Task<ResponseDataResults<List<KeyValuePair<int, string>>>> GetTemplateEmbList(string ProcessType)
        {

            ResponseDataResults<List<KeyValuePair<int, string>>> list = new ResponseDataResults<List<KeyValuePair<int, string>>>();
            list.Data = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_GetTemplateEmbList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProcessType", ProcessType));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {

                                int TemplateEmbededID = Convert.ToInt32(Convert.ToString(reader["TemplateEmbededID"]));
                                string FieldName = Convert.ToString(reader["FieldName"]);
                                list.Data.Add(new KeyValuePair<int, string>(TemplateEmbededID, FieldName));
                            }
                        }
                    }

                }
                return new ResponseDataResults<List<KeyValuePair<int, string>>>
                {
                    IsSuccess = true,
                    Message = "Data Reterival Successfully..",
                    Data = list.Data
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<KeyValuePair<int, string>>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = list.Data
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<KeyValuePair<int, string>>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = list.Data
                };
            }
        }

        public async Task<ResponseDataResults<List<Template>>> GetTemplateList(decimal GroupID)
        {
            try
            {
                List<Template> tempList = new List<Template>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_GetTemplateList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@GroupID", GroupID));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                Template temp = new Template();
                                temp.TemplateID =Convert.ToInt32(reader["TemplateID"]);
                                temp.ProjectID = Convert.ToInt32(reader["ProjectID"]);
                                temp.GroupID = Convert.ToDecimal(reader["GroupID"]);
                                temp.ProjectName = Convert.ToString(reader["ProjectName"]);
                                temp.ProcessType =Convert.ToString(reader["ProcessType"]);
                                temp.TemplateType = Convert.ToString(reader["TemplateType"]);
                                temp.TemplateMsg = Convert.ToString(reader["TemplateMsg"]);
                                temp.VendorTemplateID = Convert.ToString(reader["VendorTemplateID"]);
                                tempList.Add(temp);
                            }
                        }
                    }
                    return new ResponseDataResults<List<Template>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = tempList
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<Template>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<Template>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDataResults<int>> SaveTemplate(Template temp)
        {
            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_SaveTemplate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TemplateID", temp.TemplateID);
                        command.Parameters.AddWithValue("@GroupID", temp.GroupID);
                        command.Parameters.AddWithValue("@ProjectID", temp.ProjectID);
                        command.Parameters.AddWithValue("@ProcessType", temp.ProcessType);
                        command.Parameters.AddWithValue("@TemplateType", temp.TemplateType);
                        command.Parameters.AddWithValue("@TemplateMsg", temp.TemplateMsg);
                        command.Parameters.AddWithValue("@VendorTemplateID", temp.VendorTemplateID);
                        command.Parameters.AddWithValue("@CreatedBy", temp.CreatedBy);
                        i = await command.ExecuteNonQueryAsync();
                    }
                    return new ResponseDataResults<int>
                    {
                        IsSuccess = true,
                        Message = temp.TemplateID > 0 ? "Record updated Successfully!" : "Record Saved Successfully!",
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

 
        public async Task<ResponseDataResults<List<KeyValuePair<int, string>>>> GetProjects(decimal GroupID)
        {
           
            ResponseDataResults<List<KeyValuePair<int, string>>> list = new ResponseDataResults<List<KeyValuePair<int, string>>>();
            list.Data = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_GetProjectList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@GroupID", GroupID));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                
                                int ProjectID = Convert.ToInt32(Convert.ToString(reader["ProjectID"]));
                                string ProjectName =Convert.ToString(reader["ProjectName"]);
                                list.Data.Add(new KeyValuePair<int, string>(ProjectID, ProjectName));
                            }
                        }
                    }

                }
                return new ResponseDataResults<List<KeyValuePair<int, string>>>
                {
                    IsSuccess = true,
                    Message = "Data Reterival Successfully..",
                    Data = list.Data
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<KeyValuePair<int, string>>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = list.Data
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<KeyValuePair<int, string>>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = list.Data
                };
            }
        }
        public async Task<ResponseDataResults<int>> DeleteTemplate(int TemplateID)
        {
            int IsDelete = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_DeleteTemplate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TemplateID", TemplateID);
                        IsDelete = await command.ExecuteNonQueryAsync();
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
