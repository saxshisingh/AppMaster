using Microsoft.AspNetCore.Mvc;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace PingaUnitBooking.UI.Controllers
{
    [Route("api/StateCityController")]
    public class StateCityController : Controller
    {
        private readonly IDbInterface _dbInterface;
        public StateCityController(IDbInterface _dbInterface)
        {
            this._dbInterface = _dbInterface;
        }

        [HttpGet]
        [Route("StateList")]
        public async  Task<IActionResult> StateList()
        {
            try
            {
                List<States> _states = new List<States>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SELECT * FROM UBMSTATE", connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                 States _st = new States();
                                _st.StateID = reader.GetInt32(reader.GetOrdinal("StateID"));
                                _st.StateName = reader.GetString(reader.GetOrdinal("StateName"));
                                _states.Add(_st);
                            }
                        }
                    }
                }
                return Json(new { success = true, data= _states });
            }

            catch (Exception ex)
            {

                return Json(new { success = false, message = "Error :" + ex.Message });
            }
        }


        [HttpGet]
        [Route("CityList")]
        public async Task<IActionResult> CityList([FromQuery] int stateID)
        {
            try
            {
                List<Cities> _cities = new List<Cities>();

                using (SqlConnection connection = new SqlConnection(await _dbInterface.getREMSConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SELECT * FROM UBMCITY WHERE StateID = @stateID", connection))
                    {
                        command.Parameters.AddWithValue("@stateID", stateID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Cities _ct = new Cities();
                                    _ct.CityID = reader.GetInt32(reader.GetOrdinal("CityID"));
                                    _ct.StateID = reader.GetInt32(reader.GetOrdinal("StateID"));
                                    _ct.CityName = reader.GetString(reader.GetOrdinal("CityName"));
                                    _cities.Add(_ct);
                                }
                            }
                        }
                    }
                }
                return Json(new { success = true, data = _cities });
            }

            catch (Exception ex)
            {

                return Json(new { success = false, message = "Error :" + ex.Message });
            }
        }

    }
}
