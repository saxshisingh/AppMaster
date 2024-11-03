using Microsoft.AspNetCore.Mvc;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;


namespace PingaUnitBooking.UI.Controllers
{

    [Route("api/SchemeController")]
    public class SchemeController : Controller
    {

        private readonly ISchemeInterface appdocInterface;
        private readonly LocalStorageData _ld;
        public SchemeController(ISchemeInterface _appdocInterface, LocalStorageData _localStorage)
        {
            appdocInterface = _appdocInterface;
            this._ld = _localStorage;
        }

        [HttpPost]
        [Route("SaveScheme")]
        public async Task<IActionResult> SaveScheme([FromBody] Scheme _scheme)
        {
            try
            {
                if (_scheme == null)
                    throw new Exception("Issue in submit form!");
                _scheme.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                _scheme.CreatedBy = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await appdocInterface.SaveScheme(_scheme);
                if (!responseData.IsSuccess)
                {
                    return Json(new { success = false, message = responseData.Message });
                }
                return Json(new { success = true, message = "Scheme saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error in adding scheme: " + ex.Message });
            }
        }


        [HttpGet]
        [Route("GetSchemeList")]
        public async Task<IActionResult> SchemeumentList()
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var responseData = await appdocInterface.GetSchemeList(GroupID);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, data = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "Error to get scheme: " + ex.Message });
            }
        }


        [HttpGet]
        [Route("DeleteScheme")]
        public async Task<IActionResult> DeleteScheme(int SchemeId)
        {
            try
            {
                var responseData = await appdocInterface.DeleteScheme(SchemeId);
                if (!responseData.IsSuccess)
                {
                    return Json(new { success = false, message = responseData.Message });
                }
                return Json(new { success = true, message = "Scheme deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error to delete scheme: " + ex.Message });
            }
        }

    }
}
