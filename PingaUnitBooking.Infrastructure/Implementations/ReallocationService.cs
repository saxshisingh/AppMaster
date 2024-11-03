using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.WebSockets;
using System.Threading.Channels;

namespace PingaUnitBooking.Infrastructure.Implementations
{
    public class ReallocationService : IReallocationInterface
    {
        private readonly IDbInterface _dbInterface;
        private readonly INotificationService _notificationService;
        public ReallocationService(IDbInterface _dbInterface, INotificationService notificationService)
        {
            this._dbInterface = _dbInterface;
            _notificationService = notificationService;
        }

        public async Task<ResponseDataResults<List<KeyValuePair<int, string>>>> GetUserByRoleName(decimal GroupID, string RoleName)
        {
            ResponseDataResults<List<KeyValuePair<int, string>>> list = new ResponseDataResults<List<KeyValuePair<int, string>>>();
            list.Data = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_GetUserByRoleName", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GroupID", GroupID);
                        command.Parameters.AddWithValue("@RoleName", RoleName);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {

                                int UserID = Convert.ToInt32(Convert.ToString(reader["UserID"]));
                                string UserName = Convert.ToString(reader["UserName"]);
                                list.Data.Add(new KeyValuePair<int, string>(UserID, UserName));
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

        public async Task<ResponseDataResults<List<Reallocation>>> GetReallocationUnit(decimal GroupID, string RoleName, int UserID)
        {
            ResponseDataResults<List<Reallocation>> list = new ResponseDataResults<List<Reallocation>>();
            list.Data = new List<Reallocation>();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_GetBookedUnitByUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GroupID", GroupID);
                        command.Parameters.AddWithValue("@RoleName", RoleName);
                        command.Parameters.AddWithValue("@UserID", UserID);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                Reallocation reallocation = new Reallocation();
                                reallocation.BookingID = Convert.ToInt32(reader["BookingID"]);
                                reallocation.ProjectName = Convert.ToString(reader["ProjectName"]);
                                reallocation.TowerName = Convert.ToString(reader["TowerName"]);
                                reallocation.FloorName = Convert.ToString(reader["FloorName"]);
                                reallocation.UnitNo = Convert.ToString(reader["UnitNo"]);
                                reallocation.StatusName = Convert.ToString(reader["StatusName"]);
                                list.Data.Add(reallocation);
                            }
                        }
                    }

                }
                return new ResponseDataResults<List<Reallocation>>
                {
                    IsSuccess = true,
                    Message = "Data Reterival Successfully..",
                    Data = list.Data
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<Reallocation>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = list.Data
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<Reallocation>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = list.Data
                };
            }
        }
        public async Task<ResponseDataResults<int>> SaveBookingReallocation(decimal GroupID, int FromUserID, int ToUserID, string BookingIDs,int CreatedBy)
        {
            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_SaveBookingReallocation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure; 
                        command.Parameters.AddWithValue("@GroupID", GroupID);
                        command.Parameters.AddWithValue("@FromUserID", FromUserID);
                        command.Parameters.AddWithValue("@ToUserID", ToUserID);
                        command.Parameters.AddWithValue("@BookingIDs", BookingIDs);
                        command.Parameters.AddWithValue("@CreatedBy", CreatedBy); 
                        i = await command.ExecuteNonQueryAsync();
                    }
                    if (i > 0)
                    {
                        var emailData = await GetUserEmailData(GroupID, ToUserID);
                        TestMail testMail = new TestMail();
                        testMail.GroupID = GroupID;
                        testMail.UserID = ToUserID;
                        testMail.UbmID = int.Parse(BookingIDs);
                        testMail.ToEmail = emailData.Data[0].Value;
                        testMail.MailConfigureID = emailData.Data[0].Key;
                        testMail.Subject = "Booking Reallocate";
                        testMail.Message = "You have Assigned a Unit ";
                        await _notificationService.SendReallocationMail(testMail);
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

        public async Task<ResponseDataResults<List<KeyValuePair<int, string>>>> GetUserEmailData(decimal GroupID, int ToUserID)
        {
            ResponseDataResults<List<KeyValuePair<int, string>>> list = new ResponseDataResults<List<KeyValuePair<int, string>>>();
            list.Data = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("SELECT MU.email,UC.MailConfigureID FROM ubmUsers UU INNER JOIN mstUser MU ON MU.UserID = UU.UserID INNER JOIN ubmMailConfigure UC ON UC.GroupID = MU.GroupID and UC.ConfigureType= 'Email' WHERE UU.GroupID =1435984606 AND UU.ubmUserID=7", connection))
                    {
                        command.Parameters.AddWithValue("@UserID", ToUserID);
                        command.Parameters.AddWithValue("@GroupID", GroupID);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {

                                int MailConfigureID = Convert.ToInt32(Convert.ToString(reader["MailConfigureID"]));
                                string email = Convert.ToString(reader["email"]);
                                list.Data.Add(new KeyValuePair<int, string>(MailConfigureID, email));
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

    }
}
