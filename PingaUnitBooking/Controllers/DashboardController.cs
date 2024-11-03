using Microsoft.AspNetCore.Mvc;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;

namespace PingaUnitBooking.UI.Controllers
{
    [Route("api/DashboardController")]
    public class DashboardController : Controller
    {
        private readonly IDashboardInterface _dashboardInterface;
        private readonly LocalStorageData _localStorage;

        public DashboardController(IDashboardInterface dashboardInterface, LocalStorageData localStorage)
        {
            _dashboardInterface = dashboardInterface;
            _localStorage = localStorage;
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        [HttpGet]
        [Route("GetDashboardSummary")]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] string YearMonth)
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var UserID = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await _dashboardInterface.GetDashboardSummary(GroupID, UserID, YearMonth);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, data = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error in dashboard data: " + ex.Message });
            }
        }
        [HttpGet]
        [Route("GetUbmEmails")]
        public async Task<IActionResult> GetUbmEmails([FromQuery] int UbmID)
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var responseData = await _dashboardInterface.GetUbmEmails(GroupID, UbmID);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, data = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error to get emails: " + ex.Message });
            }
        }

    }
}
