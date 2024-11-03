using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PingaUnitBooking.Infrastructure.Helpers;

namespace PingaUnitBooking.Infrastructure.Implementations
{
    public class MailConfigureService : IMailConfigureInterface
    {
        private readonly IDbInterface _dbInterface;
        public MailConfigureService(IDbInterface _dbInterface)
        {
            this._dbInterface = _dbInterface;
        }
        public async Task<ResponseDataResults<List<MailConfigure>>> GetMailConfigure(decimal GroupID)
        {
            List<MailConfigure> mailConfigureList = new List<MailConfigure>();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_GetMailConfigure", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@GroupID", GroupID));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                MailConfigure mailConfigure = new MailConfigure();
                                mailConfigure.MailConfigureID = Convert.ToInt32(reader["MailConfigureID"]);
                                mailConfigure.GroupID = Convert.ToDecimal(reader["GroupID"]);
                                mailConfigure.ConfigureType = Convert.ToString(reader["ConfigureType"]);
                                mailConfigure.SenderName =Convert.ToString(reader["ConfigureType"]).ToUpper() =="WHATSAPP" ? Convert.ToString(reader["SenderName"]) : EnDcHelper.Base64Decode(Convert.ToString(reader["SenderName"]));
                                mailConfigure.SMTPServer = Convert.ToString(reader["SMTPServer"]);
                                mailConfigure.UserName = EnDcHelper.Base64Decode(Convert.ToString(reader["UserName"]));
                                mailConfigure.Password = EnDcHelper.Base64Decode(Convert.ToString(reader["Password"]));
                                mailConfigure.PerHourMail = Convert.ToInt32(reader["PerHourMail"]);
                                mailConfigure.SmsUrl = Convert.ToString(reader["SmsUrl"]);
                                mailConfigure.PortNo = Convert.ToInt32(reader["PortNo"]);
                                mailConfigure.TokenWA = Convert.ToString(reader["TokenWA"]);
                                mailConfigure.PhoneWA = Convert.ToString(reader["PhoneWA"]);
                                mailConfigure.BasedOn = Convert.ToString(reader["BasedOn"]);
                                mailConfigure.Provider = Convert.ToString(reader["Provider"]);
                                mailConfigure.IsActive = Convert.ToBoolean(reader["IsActive"]);
                                mailConfigureList.Add(mailConfigure);
                            }
                        }
                    }
                    return new ResponseDataResults<List<MailConfigure>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = mailConfigureList
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<MailConfigure>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = mailConfigureList
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<MailConfigure>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = mailConfigureList
                };
            }
        }
        public async Task<ResponseDataResults<int>> SaveMailConfigure(MailConfigure mailConfigure)
        {
            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_SaveMailConfigure", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MailConfigureID", mailConfigure.MailConfigureID);
                        command.Parameters.AddWithValue("@GroupID", mailConfigure.GroupID);
                        command.Parameters.AddWithValue("@ConfigureType", mailConfigure.ConfigureType);
                        command.Parameters.AddWithValue("@SenderName", mailConfigure.SenderName == "" ? "" : EnDcHelper.Base64Encode(mailConfigure.SenderName));
                        command.Parameters.AddWithValue("@SMTPServer", mailConfigure.SMTPServer);
                        command.Parameters.AddWithValue("@UserName", mailConfigure.UserName == null ? "" : EnDcHelper.Base64Encode(mailConfigure.UserName));
                        command.Parameters.AddWithValue("@Password", mailConfigure.Password == null ? "" : EnDcHelper.Base64Encode(mailConfigure.Password));
                        command.Parameters.AddWithValue("@PerHourMail", mailConfigure.PerHourMail);
                        command.Parameters.AddWithValue("@PortNo", mailConfigure.PortNo);
                        command.Parameters.AddWithValue("@SmsUrl", mailConfigure.SmsUrl);
                        command.Parameters.AddWithValue("@TokenWA", mailConfigure.TokenWA);
                        command.Parameters.AddWithValue("@PhoneWA", mailConfigure.PhoneWA);
                        command.Parameters.AddWithValue("@BasedOn", mailConfigure.BasedOn);
                        command.Parameters.AddWithValue("@Provider", mailConfigure.Provider);
                        command.Parameters.AddWithValue("@CreatedBy", mailConfigure.CreatedBy);
                        command.Parameters.AddWithValue("@IsActive", mailConfigure.IsActive);
                        i = await command.ExecuteNonQueryAsync();
                    }
                    return new ResponseDataResults<int>
                    {
                        IsSuccess = true,
                        Message = mailConfigure.MailConfigureID > 0 ? "Record updated Successfully!" : "Record Saved Successfully!",
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

        public async Task<ResponseDataResults<int>> DeleteMailConfigure(int MailConfigureID)
        {
            int IsDelete = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_DeleteMailConfigure", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MailConfigureID", MailConfigureID);
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
        public async Task<ResponseDataResults<List<Template>>> GetNotificationTemplate(decimal GroupID,int ProjectID, string ProcessType)
        {
            List<Template> templateList = new List<Template>();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_GetNotificationTemplate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@GroupID", GroupID));
                        command.Parameters.Add(new SqlParameter("@ProjectID", ProjectID));
                        command.Parameters.Add(new SqlParameter("@ProcessType", ProcessType));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                Template template = new Template();
                                template.TemplateID = Convert.ToInt32(reader["TemplateID"]);
                                template.TemplateMsg = Convert.ToString(reader["TemplateMsg"]);
                                template.VendorTemplateID = Convert.ToString(reader["VendorTemplateID"]);
                                template.TemplateType = Convert.ToString(reader["TemplateType"]);

                                templateList.Add(template);
                            }
                        }
                    }
                    return new ResponseDataResults<List<Template>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = templateList
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<Template>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = templateList
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<Template>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = templateList
                };
            }
        }
        public async Task<ResponseDataResults<Communication>> GetCustomerUnitDetail(int BookingID)
        {
            Communication communication = new Communication();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_GetCustomerUnitDetail", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BookingID", BookingID));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                communication.BookingID = Convert.ToInt32(reader["UbmID"]);
                                communication.Email = Convert.ToString(reader["CustEmail"]);
                                communication.MobileNo = Convert.ToString(reader["CustMobNo"]);
                                communication.BookingUrl = Convert.ToString(reader["BookingUrl"]);
                                communication.ApplicationType = Convert.ToString(reader["ApplicationType"]);
                                communication.UnitDetail = new UnitData();
                                communication.UnitDetail.unitNo = Convert.ToString(reader["UnitNo"]);
                                communication.UnitDetail.unitID = Convert.ToInt32(reader["UnitID"]);
                                communication.UnitDetail.area = Convert.ToDecimal(reader["Area"]);
                                communication.UnitDetail.rate = Convert.ToDecimal(reader["Rate"]);
                                communication.UnitDetail.additionalCharge = Convert.ToDecimal(reader["AdditionalAmount"]);
                                communication.UnitDetail.basicAmount = Convert.ToDecimal(reader["BasicAmount"]);
                                communication.UnitDetail.discountAmount = Convert.ToDecimal(reader["DiscountAmount"]);
                                communication.UnitDetail.unitBalconyArea = Convert.ToDecimal(reader["UnitBalconyArea"]);
                                communication.UnitDetail.unitBalconyAreaRate = Convert.ToDecimal(reader["UnitBalconyAreaRate"]);
                                communication.UnitDetail.unitCarpetArea = Convert.ToDecimal(reader["UnitCarpetArea"]);
                                communication.UnitDetail.unitCarpetAreaRate = Convert.ToDecimal(reader["UnitCarpetAreaRate"]);
                                communication.UnitDetail.projectID = Convert.ToInt32(reader["ProjectID"]);
                                communication.UnitDetail.projectName = Convert.ToString(reader["ProjectName"]);
                                communication.UnitDetail.projectAddress = Convert.ToString(reader["ProjectAddress"]);
                                communication.UnitDetail.towerName = Convert.ToString(reader["TowerName"]);
                                communication.UnitDetail.floorName = Convert.ToString(reader["FloorName"]);
                                communication.BookingAmount = Convert.ToDecimal(reader["BookingAmount"]);
                                communication.SalesPerson = Convert.ToString(reader["SalesPerson"]); 
                                communication.Password = Convert.ToString(reader["Password"]);
                            }
                        }
                    }
                    return new ResponseDataResults<Communication>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = communication
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<Communication>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = communication
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<Communication>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = communication
                };
            }
        }

        public async Task<ResponseDataResults<int>> SaveMailHistory(MailHistory mailHistory)
        {
            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_SaveMailHistory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GroupID", mailHistory.GroupID);
                        command.Parameters.AddWithValue("@UbmID", mailHistory.UbmID);
                        command.Parameters.AddWithValue("@MailConfigureID", mailHistory.MailConfigureID);
                        command.Parameters.AddWithValue("@SendTo", mailHistory.SendTo);
                        command.Parameters.AddWithValue("@Subject", mailHistory.Subject);
                        command.Parameters.AddWithValue("@Body", mailHistory.Body);
                        command.Parameters.AddWithValue("@CreatedBy", mailHistory.CreatedBy); 
                        i = await command.ExecuteNonQueryAsync();
                    }
                    return new ResponseDataResults<int>
                    {
                        IsSuccess = true,
                        Message = "Record Saved Successfully!",
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
    }
}
