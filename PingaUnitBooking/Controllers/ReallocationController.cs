using Microsoft.AspNetCore.Mvc;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;
using System.Security.AccessControl;
using static System.Net.Mime.MediaTypeNames;

namespace PingaUnitBooking.UI.Controllers
{

    [Route("api/ReallocationController")]
    public class ReallocationController : Controller
    {
        private readonly IReallocationInterface Reallocationinterface;
        private readonly INotificationService _notificationService;
        public ReallocationController(IReallocationInterface _ReallocationInterface, INotificationService notificationService)
        {
            Reallocationinterface = _ReallocationInterface;
            _notificationService = notificationService;
        }
   
        [HttpGet]
        [Route("GetUserByRoleName")]
        public async Task<IActionResult> GetUserByRoleName([FromQuery] string RoleName)
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var responseData = await Reallocationinterface.GetUserByRoleName(GroupID, RoleName);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, data = responseData.Message });

            }
            catch (Exception ex)
            {
                return Json(new { succes = false, data = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetReallocationUnit")]
        public async Task<IActionResult> GetReallocationUnit([FromQuery]   string RoleName, int UserID)
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var responseData = await Reallocationinterface.GetReallocationUnit(GroupID, RoleName,UserID);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, data = responseData.Message });

            }
            catch (Exception ex)
            {
                return Json(new { succes = false, data = ex.Message });
            }
        }

        [HttpGet]
        [Route("SaveBookingReallocation")]
        public async Task<IActionResult> SaveBookingReallocation([FromQuery] int FromUserID,int ToUserID,string BookingIds)



        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var UserID = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await Reallocationinterface.SaveBookingReallocation(GroupID, FromUserID, ToUserID, BookingIds, UserID);
                if (responseData.IsSuccess)
                {
                   return Json(new { success = true, data = responseData.Message });
                }
                return Json(new { success = false, data = responseData.Message });

            }
            catch (Exception ex)
            {
                return Json(new { succes = false, data = ex.Message });
            }
        }

      
    }
}
