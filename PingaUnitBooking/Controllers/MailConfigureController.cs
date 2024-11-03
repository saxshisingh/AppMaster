using Microsoft.AspNetCore.Mvc;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;

namespace PingaUnitBooking.UI.Controllers
{
    [Route("api/MailConfigureController")]
    public class MailConfigureController : Controller
    {


        private readonly IMailConfigureInterface mailConfigureInterface;
        private readonly INotificationService notificationService;
        private readonly LocalStorageData _ld;
        public MailConfigureController(IMailConfigureInterface _mailConfigureInterface, INotificationService _notificationService, LocalStorageData _localStorage)
        {
            mailConfigureInterface = _mailConfigureInterface;
            notificationService = _notificationService;
            this._ld = _localStorage;
        }

        [HttpPost]
        [Route("SaveMailConfigure")]
        public async Task<IActionResult> SaveMailConfigure([FromBody] MailConfigure mailConfigure)
        {
            try
            {
                if (mailConfigure == null)
                    throw new Exception("Issue in submit form!");
                mailConfigure.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                mailConfigure.CreatedBy = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await mailConfigureInterface.SaveMailConfigure(mailConfigure);
                if (!responseData.IsSuccess)
                {
                    return Json(new { success = false, message = responseData.Message });
                }
                return Json(new { success = true, message = "Mail configuration saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error in adding mail configure: " + ex.Message });
            }
        }


        [HttpGet]
        [Route("GetMailConfigure")]
        public async Task<IActionResult> GetMailConfigure()
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var responseData = await mailConfigureInterface.GetMailConfigure(GroupID);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, data = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "Error to get mail configure : " + ex.Message });
            }
        }

        [HttpGet]
        [Route("DeleteMailConfigure")]
        public async Task<IActionResult> DeleteMailConfigure(int MailConfigureID)
        {
            try
            {
                var responseData = await mailConfigureInterface.DeleteMailConfigure(MailConfigureID);
                if (!responseData.IsSuccess)
                {
                    return Json(new { success = false, message = responseData.Message });
                }
                return Json(new { success = true, message = "Configuration deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error in delete MailConfigure: " + ex.Message });
            }
        }
        [HttpPost]
        [Route("TestMailConfigure")]
        public async Task<IActionResult> TestMailConfigure([FromBody] TestMail testMail)
        {
            try
            {
                testMail.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                testMail.UserID = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await notificationService.TestMailConfigure(testMail);
                if (!responseData.IsSuccess)
                {
                    return Json(new { success = false, message = responseData.Message });
                }
                return Json(new { success = true, message = "Message sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error in sent: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("AlertMail")]
        public async Task<IActionResult> AlertMail([FromBody] TestMail testMail)
        {
            try
            {
                testMail.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                testMail.UserID = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await notificationService.AlertMail(testMail);
                if (!responseData.IsSuccess)
                {
                    return Json(new { success = false, message = responseData.Message });
                }
                return Json(new { success = true, message = "Message sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error in sent: " + ex.Message });
            }
        }


    }
}
