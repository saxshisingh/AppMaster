
using Microsoft.Extensions.Configuration;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Helpers;
using PingaUnitBooking.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace PingaUnitBooking.Infrastructure.Implementations
{
    public class AuthService : IAuthInterface
    {
        private readonly IDbInterface _dbInterface;
        public AuthService(IDbInterface _dbInterface)
        {
            this._dbInterface = _dbInterface;
        }

        public async Task<ResponseDataResults<int>> updateToken(decimal? userID, string? token, decimal? groupID)
        {
            int i = 0;
            try
            {

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_updateToken", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userID", userID);
                        command.Parameters.AddWithValue("@token", token);
                        command.Parameters.AddWithValue("@groupID", groupID);
                        i = await command.ExecuteNonQueryAsync();
                    }
                    return new ResponseDataResults<int>
                    {
                        IsSuccess = true,
                        Message = "Token Update Successfully",
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

        public async Task<ResponseDataResults<List<AuthData>>> userList(decimal? groupID, int? roleID, string type)
        {
            try
            {
                List<AuthData> authDataList = new List<AuthData>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_userList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@groupID", groupID);
                        command.Parameters.AddWithValue("@roleID", roleID);
                        command.Parameters.AddWithValue("@type", type);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                AuthData auth = new AuthData();

                                if (type == "RemsUser")
                                {
                                    auth.userId = Decimal.ToInt32(reader.GetDecimal(reader.GetOrdinal("UserID")));
                                    auth.username = reader.GetString(reader.GetOrdinal("Full Name"));
                                    auth.roleName = reader.GetString(reader.GetOrdinal("RoleName"));
                                    auth.ubRole = reader.GetString(reader.GetOrdinal("ubRoleName"));
                                    auth.isActive = reader.GetBoolean(reader.GetOrdinal("ubStatus"));
                                    auth.email = reader.GetString(reader.GetOrdinal("email"));
                                    auth.lastLoginDate = reader.GetDateTime(reader.GetOrdinal("LastLoginDate"));
                                }
                                else if (type == "UBMUser")
                                {
                                    auth.userId = Decimal.ToInt32(reader.GetInt32(reader.GetOrdinal("ubmUserID")));
                                    auth.username = reader.GetString(reader.GetOrdinal("UserName"));
                                    auth.roleName = reader.GetString(reader.GetOrdinal("RoleName"));
                                }

                                authDataList.Add(auth);
                            }
                        }
                    }
                    return new ResponseDataResults<List<AuthData>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = authDataList
                    };
                }

            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<AuthData>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<AuthData>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDataResults<AuthData>> userLogin(AuthData _auth)
        {
            AuthData auth = new AuthData();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_userLogin", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userName", _auth.email);
                        command.Parameters.AddWithValue("@password", EnDcHelper.EncryptionDecryption(_auth.password, true));

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                auth.username = reader.GetString(reader.GetOrdinal("UserName"));
                                auth.userId = reader.GetInt32(reader.GetOrdinal("UserID"));
                                auth.groupID = reader.GetDecimal(reader.GetOrdinal("GroupID"));
                                auth.email = reader.GetString(reader.GetOrdinal("email"));
                                auth.roleName = reader.GetString(reader.GetOrdinal("RoleName"));
                                auth.roleID = reader.GetDecimal(reader.GetOrdinal("RoleID"));
                                auth.UserType = reader.GetInt32(reader.GetOrdinal("UserType"));
                                auth.Credential = reader.GetString(reader.GetOrdinal("Credential"));
                                auth.CredentialInfo = reader.GetString(reader.GetOrdinal("CredentialInfo"));
                                auth.DTNullError = reader.GetString(reader.GetOrdinal("DTNullError"));
                                auth.ETADLLITEVITCA = reader.GetString(reader.GetOrdinal("ETADLLITEVITCA"));

                            }
                        }
                    }
                }
                return new ResponseDataResults<AuthData>
                {
                    IsSuccess = true,
                    Message = "Login successfully.",
                    Data = auth
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<AuthData>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<AuthData>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }
        public async Task<ResponseDataResults<int>> addUser(ubmUserData _ubmUserData)
        {
            int i = 0;
            try
            {

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_AddubmUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userID", _ubmUserData.userID);
                        command.Parameters.AddWithValue("@roleName", _ubmUserData.roleName);
                        command.Parameters.AddWithValue("@CreatedBy", _ubmUserData.CreatedBy);
                        command.Parameters.AddWithValue("@GroupID", _ubmUserData.GroupID);
                        i = await command.ExecuteNonQueryAsync();
                    }
                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Successfully Added User",
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


        public async Task<ResponseDataResults<int>> changeStatus(int? userid, decimal? groupID)
        {
            int i = 0;
            try
            {

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_ChangeUBMStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userID", userid);
                        command.Parameters.AddWithValue("@groupID", groupID);

                        i = await command.ExecuteNonQueryAsync();
                    }
                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Successfully Change Status",
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

        public async Task<ResponseDataResults<List<AuthData>>> customerAuth(AuthData auth)
        {
            try
            {
                List<AuthData> authDataList = new List<AuthData>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_CustomerLogin", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@email", auth.email);
                        command.Parameters.AddWithValue("@password", auth.password);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                AuthData _auth = new AuthData();
                                _auth.userId = reader.GetInt32(reader.GetOrdinal("CMID"));
                                _auth.groupID = reader.GetDecimal(reader.GetOrdinal("GroupID"));
                                _auth.ubmID = reader.GetInt32(reader.GetOrdinal("UbmID"));
                                authDataList.Add(_auth);
                            }
                        }
                    }
                }
                return new ResponseDataResults<List<AuthData>>
                {
                    IsSuccess = true,
                    Message = "Login successfully.",
                    Data = authDataList
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<AuthData>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<AuthData>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDataResults<List<RoleMaster>>> GetPermissions(int? userID, decimal? groupID, string? pageType)
        {
            try
            {
                List<RoleMaster> _roleMaster = new List<RoleMaster>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_getControlMaster", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@GroupID", groupID);
                        if (pageType != null) command.Parameters.AddWithValue("@pageType", pageType);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                RoleMaster _role = new RoleMaster();
                                _role.RoleName = reader.GetString(reader.GetOrdinal("RoleName"));
                                _role.MenuType = reader.GetString(reader.GetOrdinal("MenuType"));
                                _role.isTab = reader.GetBoolean(reader.GetOrdinal("isTab"));
                                _role.isCreate = reader.GetBoolean(reader.GetOrdinal("isCreate"));
                                _role.isEdit = reader.GetBoolean(reader.GetOrdinal("isEdit"));
                                _role.isView = reader.GetBoolean(reader.GetOrdinal("isView"));
                                _role.isDelete = reader.GetBoolean(reader.GetOrdinal("isDelete"));
                                _role.isApproved = reader.GetBoolean(reader.GetOrdinal("isApproved"));
                                _roleMaster.Add(_role);
                            }
                        }
                    }
                }
                return new ResponseDataResults<List<RoleMaster>>
                {
                    IsSuccess = true,
                    Message = "Fetch Permission Successfully",
                    Data = _roleMaster
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<RoleMaster>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<RoleMaster>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }
    }
}
