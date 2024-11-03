using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime;

namespace PingaUnitBooking.Infrastructure.Implementations
{
    public class BookingUnitService : IBookingInterface
    {
        private readonly IDbInterface _dbInterface;
        public BookingUnitService(IDbInterface dbInterface)
        {
            _dbInterface = dbInterface;
        }

        public async Task<ResponseDataResults<List<BookingData>>> bookingUnitList(string SearchType, string SearchText,decimal? groupID, int? userID)
        {
            try
            {
                List<BookingData> _bookData = new List<BookingData>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_GetUnitBookingList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GroupID", groupID);
                        command.Parameters.AddWithValue("@ubmUserID", userID);
                        command.Parameters.AddWithValue("@SearchType", SearchType);
                        command.Parameters.AddWithValue("@SearchText", SearchText);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                BookingData _bpp = new BookingData();
                                _bpp.ubmID = reader.GetInt32(reader.GetOrdinal("UbmID"));
                                _bpp.Status = reader.GetInt32(reader.GetOrdinal("Status"));
                                _bpp.CustomerEmail = reader.GetString(reader.GetOrdinal("CustEmail"));
                                _bpp.CustomerMobileNo = reader.GetDecimal(reader.GetOrdinal("CustMobNo"));
                                _bpp.UnitType = reader.GetString(reader.GetOrdinal("UnitType"));
                                _bpp.IsVisible = reader.GetInt32(reader.GetOrdinal("isVisible"));
                                _bpp.Remarks = reader.GetString(reader.GetOrdinal("Remarks"));
                                
                                if (_bpp.UnitType == "UnitBooking")
                                {
                                    _bpp.UnitNo = reader.GetString(reader.GetOrdinal("unitno"));
                                    _bpp.ProjectName = reader.GetString(reader.GetOrdinal("ProjectName"));
                                    _bpp.CustomerName = reader.GetString(reader.GetOrdinal("ApplicantName"));

                                    _bpp.releaseUnitDate = reader.GetDateTime(reader.GetOrdinal("ReleaseUnitDate"));
                                    _bpp.applicationType = reader.GetString(reader.GetOrdinal("ApplicationType"));
                                    _bpp.ProjectID = reader.GetDecimal(reader.GetOrdinal("ProjectID"));
                                    _bpp.TowerID = reader.GetDecimal(reader.GetOrdinal("TowerID"));
                                    _bpp.FloorID = reader.GetDecimal(reader.GetOrdinal("FloorID"));
                                    _bpp.UnitID = reader.GetInt32(reader.GetOrdinal("UnitID"));
                                    _bpp.BrokerID = reader.GetInt32(reader.GetOrdinal("BrokerID"));
                                    _bpp.BrokerDiscount = reader.GetDecimal(reader.GetOrdinal("BrokerDiscount"));
                                    _bpp.NetAmount = reader.GetDecimal(reader.GetOrdinal("NetAmount"));
                                }
                                _bookData.Add(_bpp);
                            }
                        }
                    }
                    return new ResponseDataResults<List<BookingData>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = _bookData
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<BookingData>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<BookingData>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }


        public async Task<ResponseDataResults<List<ProjectAllData>>> getProjectDataforBooking(SearchData _searchData)
        {
            try
            {
                List<ProjectAllData> _projectAllData = new List<ProjectAllData>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_getProjectDataforBooking", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@type", _searchData.type);
                        command.Parameters.AddWithValue("@getDataType", _searchData.getDataType);
                        if (_searchData.type == "Project")
                        {
                            //command.Parameters.AddWithValue("@projectID", _searchData.projectAllData.project[0].projectID);
                        }
                        else if (_searchData.type == "Tower")
                        {
                            command.Parameters.AddWithValue("@projectID", _searchData.projectAllData.project[0].projectID);
                        }
                        else if (_searchData.type == "Floor")
                        {
                            command.Parameters.AddWithValue("@towerID", _searchData.projectAllData.tower[0].towerID);
                        }
                        else if (_searchData.type == "Unit")
                        {
                            command.Parameters.AddWithValue("@floorID", _searchData.projectAllData.floor[0].floorID);
                        }
                        command.Parameters.AddWithValue("@ubmUserID", _searchData.userID);
                        command.Parameters.AddWithValue("@groupID", _searchData.groupID);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                ProjectAllData projectData = new ProjectAllData();

                                if (_searchData.type == "Project")
                                {
                                    mstProject project = new mstProject
                                    {
                                        projectID = reader.GetDecimal(reader.GetOrdinal("projectID")),
                                        projectName = reader.GetString(reader.GetOrdinal("projectName")),
                                    };
                                    projectData.project = new List<mstProject> { project };
                                }
                                else if (_searchData.type == "Tower")
                                {
                                    mstTower tower = new mstTower
                                    {
                                        towerID = reader.GetDecimal(reader.GetOrdinal("towerID")),
                                        towerName = reader.GetString(reader.GetOrdinal("towerName")),
                                    };
                                    projectData.tower = new List<mstTower> { tower };
                                }
                                else if (_searchData.type == "Floor")
                                {
                                    mstFloor floor = new mstFloor
                                    {
                                        floorID = reader.GetDecimal(reader.GetOrdinal("floorID")),
                                        floorName = reader.GetString(reader.GetOrdinal("floorName")),
                                    };
                                    projectData.floor = new List<mstFloor> { floor };
                                }
                                else if (_searchData.type == "Unit")
                                {
                                    mstunit unit = new mstunit
                                    {
                                        unitID = reader.GetDecimal(reader.GetOrdinal("unitID")),
                                        unitName = reader.GetString(reader.GetOrdinal("unitName")),
                                    };
                                    projectData.unitData = new List<mstunit> { unit };
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

        public async Task<ResponseDataResults<BookingData>> addBookedUnit(BookingData _bookingData)
        {
            try
            {
                BookingData bd = new BookingData();
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_AddubmUnitBooking", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (_bookingData.UnitType == "UnitBooking")
                        {
                            command.Parameters.AddWithValue("@UnitID", _bookingData.UnitID);
                            command.Parameters.AddWithValue("@releaseUnitDate", _bookingData.releaseUnitDate);
                            command.Parameters.AddWithValue("@applicationType", _bookingData.applicationType);
                            command.Parameters.AddWithValue("@BrokerID", _bookingData.BrokerID);
                            command.Parameters.AddWithValue("@brokerDiscount", _bookingData.BrokerDiscount);

                        }
                        command.Parameters.AddWithValue("@UbookmID", _bookingData.ubmID);
                        command.Parameters.AddWithValue("@Remarks", _bookingData.Remarks);
                        command.Parameters.AddWithValue("@UnitType", _bookingData.UnitType);
                        command.Parameters.AddWithValue("@cutomerMobileNo", _bookingData.CustomerMobileNo);
                        command.Parameters.AddWithValue("@customerEmail", _bookingData.CustomerEmail);

                        command.Parameters.AddWithValue("@createdBy", _bookingData.createdBy);
                        command.Parameters.AddWithValue("@groupID", _bookingData.groupID);


                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            { 
                                bd.ubmID = reader.GetInt32(reader.GetOrdinal("ubmID"));
                                 
                            }
                        }
                    }
                }
                return new ResponseDataResults<BookingData>
                {
                    IsSuccess = true,
                    Message = "Successfully Booked Unit",
                    Data = bd
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<BookingData>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<BookingData>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDataResults<int>> addPaymentModel(PaymentModel _paymentModel)
        {
            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_AddPaymentDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@UbmID", _paymentModel.UbmID);
                        command.Parameters.AddWithValue("@PaymentMode", _paymentModel.PaymentMode);
                        command.Parameters.AddWithValue("@PaymentDate", _paymentModel.PaymentDate);
                        command.Parameters.AddWithValue("@ChequeNo", _paymentModel.ChequeNo);
                        command.Parameters.AddWithValue("@BankName", _paymentModel.BankName);
                        command.Parameters.AddWithValue("@TransactionNo", _paymentModel.TransactionNo); ;
                        command.Parameters.AddWithValue("@Amount", _paymentModel.Amount);
                        command.Parameters.AddWithValue("@CreatedBy", _paymentModel.CreatedBy);
                        command.Parameters.AddWithValue("@GroupID", _paymentModel.GroupID);
                        i = await command.ExecuteNonQueryAsync();
                    }

                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Successfully Added Payment Details",
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

        public async Task<ResponseDataResults<List<PaymentModel>>> getPaymentModelList(decimal? groupID, int? ubmID)
        {
            try
            {
                List<PaymentModel> _paymentModelList = new List<PaymentModel>();
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_getpaymentDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@GroupID", groupID);
                        command.Parameters.AddWithValue("@ubmId", ubmID);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                PaymentModel bd = new PaymentModel();
                                bd.UbmPaymentId = reader.GetInt32(reader.GetOrdinal("UbmPaymentId"));
                                bd.UbmID = reader.GetInt32(reader.GetOrdinal("UbmID"));
                                bd.PaymentMode = reader.GetString(reader.GetOrdinal("PaymentMode"));
                                bd.FormatPaymentDate = reader.GetString(reader.GetOrdinal("PaymentDate"));
                                bd.ChequeNo = reader.GetString(reader.GetOrdinal("ChequeNo"));
                                bd.BankName = reader.GetString(reader.GetOrdinal("BankName"));
                                bd.TransactionNo = reader.GetString(reader.GetOrdinal("TransactionNo"));
                                bd.Amount = reader.GetDecimal(reader.GetOrdinal("Amount"));
                                _paymentModelList.Add(bd);
                            }
                        }
                    }
                }
                return new ResponseDataResults<List<PaymentModel>>
                {
                    IsSuccess = true,
                    Message = "Data Retrive Successfully",
                    Data = _paymentModelList
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<PaymentModel>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<PaymentModel>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDataResults<List<ApplicationDoc>>> getApplicantDocument(decimal? groupID, int? ubmID)
        {
            try
            {
                List<ApplicationDoc> _applicantDocList = new List<ApplicationDoc>();
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_ApplicantDocList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@GroupID", groupID);
                        command.Parameters.AddWithValue("@UbmID", ubmID);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                ApplicationDoc ad = new ApplicationDoc();
                                ad.UbmKycid = reader.GetInt32(reader.GetOrdinal("UbmKycid"));
                                ad.DocumentName = reader.GetString(reader.GetOrdinal("DocumentName"));
                                ad.DocumentID = reader.GetInt32(reader.GetOrdinal("DocumentID"));
                                ad.Mandatory = reader.GetBoolean(reader.GetOrdinal("IsMandatory"));
                                ad.DocumentUrl = reader.GetString(reader.GetOrdinal("DocUrl"));
                                ad.MobileNo = reader.GetDecimal(reader.GetOrdinal("CustMobNo"));
                                ad.unitID = reader.GetInt32(reader.GetOrdinal("UnitID"));


                                _applicantDocList.Add(ad);
                            }
                        }
                    }
                }
                return new ResponseDataResults<List<ApplicationDoc>>
                {
                    IsSuccess = true,
                    Message = "Data Retrive Successfully",
                    Data = _applicantDocList
                };
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

        public async Task<ResponseDataResults<int>> addApplicantDocuments(ApplicationDoc _doc)
        {
            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_AddApplicantKYCDocuments", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@UbmID", _doc.UbmID);
                        command.Parameters.AddWithValue("@DocID", _doc.DocumentID);
                        command.Parameters.AddWithValue("@DocUrl", _doc.DocumentUrl);
                        command.Parameters.AddWithValue("@GroupID", _doc.GroupID);
                        i = await command.ExecuteNonQueryAsync();
                    }

                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Successfully Added Document",
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

        public async Task<ResponseDataResults<int>> DeleteAttachments(ApplicationDoc _doc)
        {
            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_deleteKYCDocuments", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@UbmKycid", _doc.UbmKycid);
                        command.Parameters.AddWithValue("@GroupID", _doc.GroupID);
                        i = await command.ExecuteNonQueryAsync();
                    }

                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Successfully Delete Document",
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

        public async Task<ResponseDataResults<int>> addApplicantDetails(ApplicantData _applicantData)
        {
            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_AddApplicant", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@AplicantID", _applicantData.ApplicantID);
                        command.Parameters.AddWithValue("@ApplicantType", _applicantData.ApplicantType);
                        /*   command.Parameters.AddWithValue("@ApplicantionDate", _applicantData.ApplicantUnitDate);*/
                        command.Parameters.AddWithValue("@ApplicantName", _applicantData.ApplicantBuyerName);
                        command.Parameters.AddWithValue("@SO", _applicantData.Applicantswd);
                        command.Parameters.AddWithValue("@Dob", _applicantData.Applicantdob);
                        command.Parameters.AddWithValue("@Age", _applicantData.ApplicantAge);
                        command.Parameters.AddWithValue("@Nationality", _applicantData.ApplicantNationality);
                        command.Parameters.AddWithValue("@Occupation", _applicantData.ApplicantOccupation);
                        command.Parameters.AddWithValue("@Pan", _applicantData.ApplicantPan);
                        command.Parameters.AddWithValue("@Aadharno", _applicantData.ApplicantAdharNo);
                        command.Parameters.AddWithValue("@Address", _applicantData.ApplicantCorrAddress);
                        command.Parameters.AddWithValue("@CityId", _applicantData.ApplicantCityID);
                        command.Parameters.AddWithValue("@StateId", _applicantData.ApplicantStateID);
                        command.Parameters.AddWithValue("@CountryName", _applicantData.ApplicantCountry);
                        command.Parameters.AddWithValue("@Pin", _applicantData.ApplicantPIN);
                        command.Parameters.AddWithValue("@Email", _applicantData.ApplicantEmail);
                        command.Parameters.AddWithValue("@Phone2", _applicantData.ApplicantMob);
                        command.Parameters.AddWithValue("@GroupID", _applicantData.GroupID);
                        command.Parameters.AddWithValue("@UbmID", _applicantData.UbmID);
                        command.Parameters.AddWithValue("@CreatedBy", _applicantData.CreatedBy);
                        i = await command.ExecuteNonQueryAsync();
                    }
                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Successfully Added Applicant Details",
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

        public async Task<ResponseDataResults<List<ApplicantData>>> getApplicantList(decimal? groupID, int? ubmID, int? appType)
        {
            try
            {
                List<ApplicantData> _applicantList = new List<ApplicantData>();
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_ApplicantList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@GroupID", groupID);
                        command.Parameters.AddWithValue("@ApplicantType", appType);
                        command.Parameters.AddWithValue("@UbmID", ubmID);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                ApplicantData ad = new ApplicantData();
                                ad.ApplicantID = reader.GetInt32(reader.GetOrdinal("UbmApplicantID"));
                                ad.ApplicantBuyerName = reader.GetString(reader.GetOrdinal("ApplicantName"));
                                ad.ApplicantType = reader.GetInt32(reader.GetOrdinal("ApplicantType"));
                                ad.Applicantswd = reader.GetString(reader.GetOrdinal("SO"));
                                ad.Applicantdob = reader.GetDateTime(reader.GetOrdinal("Dob"));
                                ad.ApplicantAge = reader.GetDecimal(reader.GetOrdinal("Age"));
                                ad.ApplicantNationality = reader.GetString(reader.GetOrdinal("Nationality"));
                                ad.ApplicantOccupation = reader.GetString(reader.GetOrdinal("Occupation"));
                                ad.ApplicantPan = reader.GetString(reader.GetOrdinal("Pan"));
                                ad.ApplicantAdharNo = reader.GetString(reader.GetOrdinal("Aadharno"));
                                ad.ApplicantCorrAddress = reader.GetString(reader.GetOrdinal("Address"));
                                ad.ApplicantCityID = reader.GetInt32(reader.GetOrdinal("CityId"));
                                ad.ApplicantStateID = reader.GetInt32(reader.GetOrdinal("StateId"));
                                ad.ApplicantCountry = reader.GetString(reader.GetOrdinal("CountryName"));
                                ad.ApplicantPIN = reader.GetString(reader.GetOrdinal("Pin"));
                                ad.ApplicantEmail = reader.GetString(reader.GetOrdinal("Email"));
                                ad.ApplicantMob = reader.GetDecimal(reader.GetOrdinal("Phone2"));
                                ad.ApplicantUnitDate = reader.GetDateTime(reader.GetOrdinal("ApplicantionDate"));

                                _applicantList.Add(ad);
                            }
                        }
                    }
                }
                return new ResponseDataResults<List<ApplicantData>>
                {
                    IsSuccess = true,
                    Message = "Data Retrive Successfully",
                    Data = _applicantList
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<ApplicantData>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<ApplicantData>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDataResults<int>> deleteCoApplicant(ApplicantData _applicantData)
        {
            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_DeleteApplicant", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@UbmApplicantID", _applicantData.ApplicantID);
                        command.Parameters.AddWithValue("@GroupID", _applicantData.GroupID);
                        i = await command.ExecuteNonQueryAsync();
                    }

                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Successfully Delete Document",
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

        public async Task<ResponseDataResults<int>> ChangeUbmStatus(UbmStatus _UbmStatus)
        {

            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_ChangeUbmAuthorization", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@UbmID", _UbmStatus.UbmID);
                        command.Parameters.AddWithValue("@GroupID", _UbmStatus.GroupID);
                        command.Parameters.AddWithValue("@CreatedBy", _UbmStatus.CreatedBy);
                        command.Parameters.AddWithValue("@Remarks", _UbmStatus.Remarks);
                        command.Parameters.AddWithValue("@AuthorizationDropdown", _UbmStatus.authorizationDropdown);
                        i = await command.ExecuteNonQueryAsync();
                    }

                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Successfully Change the Status",
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

        public async Task<ResponseDataResults<int>> ChangeUbmAuthorization(UbmStatus _UbmStatus)
        {

            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_ChangeUbmAuthorizationClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@UbmID", _UbmStatus.UbmID);
                        command.Parameters.AddWithValue("@GroupID", _UbmStatus.GroupID);
                        command.Parameters.AddWithValue("@CreatedBy", _UbmStatus.CreatedBy);
                        command.Parameters.AddWithValue("@Remarks", _UbmStatus.Remarks);
                        i = await command.ExecuteNonQueryAsync();
                    }

                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Successfully Sent to Authorization",
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

        public async Task<ResponseDataResults<List<BookingData>>> ubmDetailsByUnitID(decimal? groupID, int? ubmID)
        {
            try
            {
                List<BookingData> _bookData = new List<BookingData>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_GetDetailsByUnitID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GroupID", groupID);
                        command.Parameters.AddWithValue("@ubmID", ubmID);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                BookingData _bpp = new BookingData();
                                _bpp.ubmID = reader.GetInt32(reader.GetOrdinal("UbmID"));
                                _bpp.Status = reader.GetInt32(reader.GetOrdinal("Status"));
                                _bpp.CustomerEmail = reader.GetString(reader.GetOrdinal("CustEmail"));
                                _bpp.CustomerMobileNo = reader.GetDecimal(reader.GetOrdinal("CustMobNo"));
                                _bpp.UnitType = reader.GetString(reader.GetOrdinal("UnitType"));
                                _bpp.UnitNo = reader.GetString(reader.GetOrdinal("unitno"));
                                _bpp.ProjectName = reader.GetString(reader.GetOrdinal("ProjectName"));
                                _bpp.CustomerName = reader.GetString(reader.GetOrdinal("ApplicantName"));

                                _bpp.releaseUnitDate = reader.GetDateTime(reader.GetOrdinal("ReleaseUnitDate"));
                                _bpp.applicationType = reader.GetString(reader.GetOrdinal("ApplicationType"));
                                _bpp.ProjectID = reader.GetDecimal(reader.GetOrdinal("ProjectID"));
                                _bpp.TowerID = reader.GetDecimal(reader.GetOrdinal("TowerID"));
                                _bpp.TowerName = reader.GetString(reader.GetOrdinal("TowerName"));
                                _bpp.FloorID = reader.GetDecimal(reader.GetOrdinal("FloorID"));
                                _bpp.FloorName = reader.GetString(reader.GetOrdinal("FloorName"));
                                _bpp.UnitID = reader.GetInt32(reader.GetOrdinal("UnitID"));
                                _bookData.Add(_bpp);
                            }
                        }
                    }
                    return new ResponseDataResults<List<BookingData>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = _bookData
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<BookingData>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<BookingData>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }


        public async Task<ResponseDataResults<List<string>>> GetApplicationDocument(decimal? groupID, string ApplicationType)
        {
            try
            {
                List<string> _docList = new List<string>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_GetApplicationDocument", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GroupID", groupID);
                        command.Parameters.AddWithValue("@ApplicationType", ApplicationType);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                string docname = Convert.ToString(reader["DocumentName"]);
                                _docList.Add(docname);
                            }
                        }
                    }
                    return new ResponseDataResults<List<string>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = _docList
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<string>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<string>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }

        public async  Task<ResponseDataResults<int>> DeletePaymentPlan(decimal? groupID, decimal? paymentID)
        {
            int i = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_deleteBookingAmount", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@groupID", groupID);
                        command.Parameters.AddWithValue("@paymentID", paymentID);
                        i = await command.ExecuteNonQueryAsync();
                    }

                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Successfully Delete Payment Plan",
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

        public async Task<ResponseDataResults<string>> getTncTemplate(decimal groupID, decimal? ubmID)
        {
            try
            {
                string template="";

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_TNCTemplate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@groupID", groupID);
                        command.Parameters.AddWithValue("@ubmID", ubmID);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {

                            while (reader.Read())
                            {
                                template = Convert.ToString(reader["TemplateMsg"]);
                            }
                            
                        }
                    }
                    return new ResponseDataResults<string>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = template
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

        //ADDED BY AMIT
        public async Task<ResponseDataResults<UnitInfo>> GetUnitInfo(decimal? groupID,int UnitID)
        {
            try
            {
                UnitInfo unitInfo = new UnitInfo();
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_GetUnitDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GroupID", groupID);
                        command.Parameters.AddWithValue("@UnitID", UnitID);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                unitInfo.BlockName = Convert.ToString(reader["BlockName"]);
                                unitInfo.UnitNo = Convert.ToString(reader["UnitNo"]);
                                unitInfo.UnitTypeName = Convert.ToString(reader["UnitTypeName"]);
                                unitInfo.UnitCarpetArea = Convert.ToDecimal(reader["UnitCarpetArea"]);
                                unitInfo.SchemeName = Convert.ToString(reader["SchemeName"]);
                                unitInfo.IntPlanName = Convert.ToString(reader["IntPlanName"]);
                                unitInfo.PayPlanName = Convert.ToString(reader["PayPlanName"]);
                                unitInfo.PayPlanID = Convert.ToInt32(reader["PayPlanID"]);
                            }
                        }
                    }
                    return new ResponseDataResults<UnitInfo>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = unitInfo
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<UnitInfo>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<UnitInfo>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDataResults<List<UnitLogs>>> GetUnitLogs(decimal? groupID, int? ubmID)
        {
            try
            {

                List<UnitLogs> unitLogs = new List<UnitLogs>();
              
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_getUbmUnitLogs", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GroupID", groupID);
                        command.Parameters.AddWithValue("@ubmID", ubmID);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                UnitLogs _unit = new UnitLogs();
                                _unit.StatusName = Convert.ToString(reader["StatusName"]);
                                _unit.Remarks = Convert.ToString(reader["Remarks"]);
                                _unit.CreatedAt = Convert.ToString(reader["CreatedOn"]);
                                _unit.UserName = Convert.ToString(reader["UserName"]);
                                unitLogs.Add(_unit);
                            }
                        }
                       
                        return new ResponseDataResults<List<UnitLogs>>
                        {
                            IsSuccess = true,
                            Message = "Data Reterival Successfully..",
                            Data = unitLogs
                        };
                    }
                }
            }

            catch (SqlException ex)
            {
                return new ResponseDataResults<List<UnitLogs>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<UnitLogs>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDataResults<List<KeyValuePair<int, string>>>> GetBrokerList(decimal GroupID)
        {

            ResponseDataResults<List<KeyValuePair<int, string>>> list = new ResponseDataResults<List<KeyValuePair<int, string>>>();
            list.Data = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_getUbmbrokerList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@GroupID", GroupID));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                int BrokerID = Convert.ToInt32(Convert.ToString(reader["BrokerID"]));
                                string BrokerName = Convert.ToString(reader["BrokerName"]);
                                list.Data.Add(new KeyValuePair<int, string>(BrokerID, BrokerName));
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
