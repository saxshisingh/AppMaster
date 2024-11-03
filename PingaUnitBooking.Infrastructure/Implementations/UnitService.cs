using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;

namespace PingaUnitBooking.Infrastructure.Implementations
{
    public class UnitService : IUnitInterface
    {
        private readonly IDbInterface _dbInterface;
        public UnitService(IDbInterface _dbInterface)
        {
            this._dbInterface = _dbInterface;
        }

        public async Task<ResponseDataResults<List<UnitData>>> unitDetailsList(decimal? groupID, int? userID, int? ProjectID, int? TowerID , string statusType)
        {
            try
            {
                List<UnitData> _UnitData = new List<UnitData>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_unitDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@groupID", groupID);
                        command.Parameters.AddWithValue("@userID", userID);
                        command.Parameters.AddWithValue("@ProjectID", ProjectID);
                        command.Parameters.AddWithValue("@TowerID", TowerID);
                        command.Parameters.AddWithValue("@statusType", statusType);
                        

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                UnitData _ud = new UnitData();

                                _ud.towerName = reader.GetString(reader.GetOrdinal("TowerName"));
                                _ud.companyID = reader.GetDecimal(reader.GetOrdinal("CompanyID"));
                                _ud.locationID = reader.GetDecimal(reader.GetOrdinal("LocationID"));
                                _ud.floorID = reader.GetDecimal(reader.GetOrdinal("FloorID"));
                                _ud.floorName = reader.GetString(reader.GetOrdinal("FloorName"));
                                _ud.projectName = reader.GetString(reader.GetOrdinal("ProjectName"));
                                _ud.locationName = reader.GetString(reader.GetOrdinal("LocationName"));
                                _ud.unitID = reader.GetDecimal(reader.GetOrdinal("UnitID"));
                                _ud.unitNo = reader.GetString(reader.GetOrdinal("UnitNo"));
                                _ud.categoryName = reader.GetString(reader.GetOrdinal("CatName"));
                                _ud.netAmount = reader.GetDecimal(reader.GetOrdinal("NetAmount"));
                                _ud.area = reader.GetDecimal(reader.GetOrdinal("area"));
                                _ud.rate = reader.GetDecimal(reader.GetOrdinal("Rate"));
                                _ud.unitSuperArea = reader.GetDecimal(reader.GetOrdinal("UnitSuperArea"));
                                _ud.unitSuperAreaRate = reader.GetDecimal(reader.GetOrdinal("UnitSuperAreaRate"));
                                _ud.unitBuiltUpArea = reader.GetDecimal(reader.GetOrdinal("UnitBuiltUpArea"));
                                _ud.unitBuiltUpAreaRate = reader.GetDecimal(reader.GetOrdinal("UnitBuiltUpAreaRate"));
                                _ud.unitTerraceArea = reader.GetDecimal(reader.GetOrdinal("UnitTerraceArea"));
                                _ud.unitTerraceAreaRate = reader.GetDecimal(reader.GetOrdinal("UnitTerraceAreaRate"));
                                _ud.unitBalconyArea = reader.GetDecimal(reader.GetOrdinal("UnitBalconyArea"));
                                _ud.unitBalconyAreaRate = reader.GetDecimal(reader.GetOrdinal("UnitBalconyAreaRate"));
                                _ud.basicAmount = reader.GetDecimal(reader.GetOrdinal("BasicAmount"));
                                _ud.unitCarpetArea = reader.GetDecimal(reader.GetOrdinal("unitCarpetArea"));
                                _ud.unitStatus = reader.GetInt32(reader.GetOrdinal("unitStatus"));
                                _ud.additionalCharge = reader.GetDecimal(reader.GetOrdinal("AdditionalCharge"));
                                _ud.discountAmount = reader.GetDecimal(reader.GetOrdinal("DiscountAmount"));
                                _ud.minSaleAmount = reader.GetDecimal(reader.GetOrdinal("minSaleAmount"));
                                _ud.maxSaleAmount = reader.GetDecimal(reader.GetOrdinal("maxSaleAmount"));
                                _ud.payplanID = reader.GetInt32(reader.GetOrdinal("PayPlanID"));
                                _ud.intPlanID = reader.GetInt32(reader.GetOrdinal("IntPlanID"));
                                _ud.SchemeID = reader.GetInt32(reader.GetOrdinal("SchemeID"));
                                _ud.SchemeName = Convert.ToString(reader["SchemeName"]);
                                _ud.CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy"));

                                _UnitData.Add(_ud);
                            }
                        }
                    }
                    return new ResponseDataResults<List<UnitData>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = _UnitData
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<UnitData>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<UnitData>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }

        }

        public async Task<ResponseDataResults<List<paymentPlan>>> paymentPlanList(decimal? blockID, decimal? unitID, decimal? companyID, decimal? locationID)
        {
            try
            {
                List<paymentPlan> plans = new List<paymentPlan>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_GetPaymentPlanByUnitBlock", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BlockID", blockID);
                        command.Parameters.AddWithValue("@UnitID", unitID);
                        command.Parameters.AddWithValue("@CompanyID", companyID);
                        command.Parameters.AddWithValue("@LocationID", locationID);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                paymentPlan _planData = new paymentPlan();

                                _planData.payplanID = reader.IsDBNull(reader.GetOrdinal("PayplanID")) ? 0 : reader.GetDecimal(reader.GetOrdinal("PayplanID"));
                                _planData.payplanName = reader.GetString(reader.GetOrdinal("PayplanName"));


                                plans.Add(_planData);
                            }
                        }
                    }
                    return new ResponseDataResults<List<paymentPlan>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = plans
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<paymentPlan>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<paymentPlan>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }

        }

        public async Task<ResponseDataResults<List<intrestPlan>>> intrestPlanList(decimal? companyID, decimal? locationID, decimal? groupID)
        {
            try
            {
                List<intrestPlan> intrests = new List<intrestPlan>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("GetFillIntrestPlan", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CompanyID", companyID);
                        command.Parameters.AddWithValue("@LocationID", locationID);
                        command.Parameters.AddWithValue("@GroupID", groupID);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                intrestPlan _intrestData = new intrestPlan();

                                _intrestData.intPlanID = reader.GetDecimal(reader.GetOrdinal("IntPlanID"));
                                _intrestData.intPlanName = reader.GetString(reader.GetOrdinal("IntPlanName"));

                                intrests.Add(_intrestData);
                            }
                        }
                    }
                    return new ResponseDataResults<List<intrestPlan>>
                    {
                        IsSuccess = true,
                        Message = "Data Reterival Successfully..",
                        Data = intrests
                    };
                }
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<intrestPlan>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<intrestPlan>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }

        }

        public async Task<ResponseDataResults<int>> addUbmUnit(UnitData _unitData)
        {
            int i = 0;
            try
            {

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_AddUnitDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UnitID", _unitData.unitID);
                        command.Parameters.AddWithValue("@BasicAmount", _unitData.basicAmount);
                        command.Parameters.AddWithValue("@AdditionalAmount", _unitData.additionalCharge);
                        command.Parameters.AddWithValue("@DiscountAmount", _unitData.discountAmount);
                        command.Parameters.AddWithValue("@NetAmount", _unitData.netAmount);
                        command.Parameters.AddWithValue("@minSaleAmount", _unitData.minSaleAmount);
                        command.Parameters.AddWithValue("@maxSaleAmount", _unitData.maxSaleAmount);
                        command.Parameters.AddWithValue("@IntPlanID", _unitData.intrest.intPlanID);
                        command.Parameters.AddWithValue("@PayPlanID", _unitData.payment.payplanID);
                        command.Parameters.AddWithValue("@SchemeID", _unitData.SchemeID);
                        command.Parameters.AddWithValue("@CreatedBy", _unitData.userID);
                        command.Parameters.AddWithValue("@groupID", _unitData.groupID);

                        i = await command.ExecuteNonQueryAsync();
                    }
                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Sent for Approval",
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
        public async Task<ResponseDataResults<List<KeyValuePair<int, string>>>> GetUserProjects(decimal GroupId, int ubmUserId)
        {

            ResponseDataResults<List<KeyValuePair<int, string>>> list = new ResponseDataResults<List<KeyValuePair<int, string>>>();
            list.Data = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_GetUserProjects", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@GroupID", GroupId));
                        command.Parameters.Add(new SqlParameter("@ubmUserId", ubmUserId));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {

                                int ProjectID = Convert.ToInt32(Convert.ToString(reader["ProjectID"]));
                                string ProjectName = Convert.ToString(reader["ProjectName"]);
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
        public async Task<ResponseDataResults<List<KeyValuePair<int, string>>>> GetTowerByProjectId(decimal GroupId, int ubmUserId, int ProjectID)
        {

            ResponseDataResults<List<KeyValuePair<int, string>>> list = new ResponseDataResults<List<KeyValuePair<int, string>>>();
            list.Data = new List<KeyValuePair<int, string>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_GetTowerByProjectId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@GroupID", GroupId));
                        command.Parameters.Add(new SqlParameter("@ProjectID", ProjectID));
                        command.Parameters.Add(new SqlParameter("@ubmUserId", ubmUserId));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                int BlockID = Convert.ToInt32(Convert.ToString(reader["BlockID"]));
                                string BlockName = Convert.ToString(reader["BlockName"]);
                                list.Data.Add(new KeyValuePair<int, string>(BlockID, BlockName));
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


        public async Task<ResponseDataResults<int>> changeUnitStatus(int? unitID, decimal? groupID, int? status)
        {
            int i = 0;
            try
            {

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ubm_changeUnitStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UnitID", unitID);
                        command.Parameters.AddWithValue("@GroupID", groupID);
                        command.Parameters.AddWithValue("@status", status);

                        i = await command.ExecuteNonQueryAsync();
                    }
                }
                return new ResponseDataResults<int>
                {
                    IsSuccess = true,
                    Message = "Unit Status Changed Successfully",
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

        public async Task<ResponseDataResults<List<PayPlan>>> GetPaymentPlan(int PayplanID)
        {
            ResponseDataResults<List<PayPlan>> list = new ResponseDataResults<List<PayPlan>>();
            list.Data = new List<PayPlan>();
            try
            {
                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ubm_GetPayPlanById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@PayPlanID", PayplanID));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                PayPlan payPlan = new PayPlan();
                                payPlan.StageID = Convert.ToInt32(Convert.ToString(reader["StageID"]));
                                payPlan.StageName = Convert.ToString(reader["StageName"]);
                                payPlan.ChargeName = Convert.ToString(reader["ChargeName"]);
                                payPlan.DuePercentage = Convert.ToDecimal(reader["DuePercentage"]);
                                list.Data.Add(payPlan);
                            }
                        }
                    }
                }
                return new ResponseDataResults<List<PayPlan>>
                {
                    IsSuccess = true,
                    Message = "Data Reterival Successfully..",
                    Data = list.Data
                };
            }
            catch (SqlException ex)
            {
                return new ResponseDataResults<List<PayPlan>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = list.Data
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataResults<List<PayPlan>>
                {
                    IsSuccess = false,
                    Message = "An error occurred: " + ex.Message,
                    Data = list.Data
                };
            }
        }


        public async Task<ResponseDataResults<List<KeyValuePair<int, string>>>> GetScheme(decimal GroupID)
        {

            ResponseDataResults<List<KeyValuePair<int, string>>> list = new ResponseDataResults<List<KeyValuePair<int, string>>>();
            list.Data = new List<KeyValuePair<int, string>>();
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
                                int SchemeID = Convert.ToInt32(Convert.ToString(reader["SchemeID"]));
                                string SchemeName = Convert.ToString(reader["SchemeName"]);
                                list.Data.Add(new KeyValuePair<int, string>(SchemeID, SchemeName));
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
