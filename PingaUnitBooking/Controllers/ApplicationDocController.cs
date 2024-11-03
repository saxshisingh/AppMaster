using Microsoft.AspNetCore.Mvc;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;


namespace PingaUnitBooking.UI.Controllers
{

    [Route("api/ApplicationDocController")]
    public class ApplicationDocController : Controller
    {

        private readonly IApplicationDocInterface appdocInterface;
        private readonly LocalStorageData _ld;
        public ApplicationDocController(IApplicationDocInterface _appdocInterface, LocalStorageData _localStorage)
        {
            appdocInterface = _appdocInterface;
            this._ld = _localStorage;
        }

        [HttpPost]
        [Route("SaveApplicationDocument")]
        public async Task<IActionResult> SaveApplicationDocument([FromBody] ApplicationDoc _ubmappdoc)
        {
            try
            {
                if (_ubmappdoc == null)
                    throw new Exception("Issue in submit form!");
                _ubmappdoc.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                _ubmappdoc.UserID = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await appdocInterface.SaveApplicationDocument(_ubmappdoc);
                if (!responseData.IsSuccess)
                {
                    return Json(new { success = false, message = responseData.Message });
                }
                return Json(new { success = true, message = "Application document saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error in adding documnet: " + ex.Message });
            }
        }


        [HttpGet]
        [Route("GetApplicationDocList")]
        public async Task<IActionResult> ApplicationDocumentList()
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var responseData = await appdocInterface.GetApplicationDocList(GroupID);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, data = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "Error to get documnet: " + ex.Message });
            }
        }


        [HttpGet]
        [Route("DeleteApplicationDocument")]
        public async Task<IActionResult> DeleteApplicationDocument(int id)
        {
            try
            {
                var responseData = await appdocInterface.DeleteApplicationDocument(id);
                if (!responseData.IsSuccess)
                {
                    return Json(new { success = false, message = responseData.Message });
                }
                return Json(new { success = true, message = "Application document deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error to delete documnet: " + ex.Message });
            }
        }

    }
}
