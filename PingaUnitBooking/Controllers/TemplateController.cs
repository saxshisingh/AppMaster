using Microsoft.AspNetCore.Mvc;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;
using System.Security.AccessControl;
using static System.Net.Mime.MediaTypeNames;

namespace PingaUnitBooking.UI.Controllers
{

    [Route("api/TemplateController")]
    public class TemplateController : Controller
    {
        private readonly ITemplateInterface templateinterface;
        private readonly LocalStorageData _ld;
        
        public TemplateController(ITemplateInterface _templateInterface, LocalStorageData _localStorage)
        {
            templateinterface = _templateInterface;
            this._ld = _localStorage;
        }



        [HttpGet]
        [Route("ProcessTypeList")]
        public async Task<IActionResult> ProcessTypeList()
        {
            try
            {
                var responseData = await templateinterface.ProcessTypeList();
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
        [Route("GetAppDocList")]
        public async Task<IActionResult> GetAppDocList(string ApplicationType)
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
               
                var responseData = await templateinterface.GetAppDocList(ApplicationType, GroupID);
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
        [Route("GetTemplateEmbList")]
        public async Task<IActionResult> GetTemplateEmbList([FromQuery] string ProcessType)
        {
            try
            {
                var responseData = await templateinterface.GetTemplateEmbList(ProcessType);
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
        [Route("GetProjects")]
        public async Task<IActionResult> GetProjects()
        {
            try
            {

                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var responseData = await templateinterface.GetProjects(GroupID);
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
        [Route("GetTemplateList")]
        public async Task<IActionResult> ApplicationDocumentList()
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var responseData = await templateinterface.GetTemplateList(GroupID);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, data = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = "Error in adding documnet: " + ex.Message });
            }
        }
        [HttpPost]
        [Route("SaveTemplate")]
        public async Task<IActionResult> SaveTemplate([FromBody] Template _temp)
        {
            try
            {
                if (_temp == null)
                    throw new Exception("Issue in template!");
                _temp.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                _temp.CreatedBy = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await templateinterface.SaveTemplate(_temp);
                if (!responseData.IsSuccess)
                {
                    return Json(new { success = false, message = responseData.Message });
                }
                return Json(new { success = true, message = "Template saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error in adding documnet: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("DeleteTemplate")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            try
            {
                var responseData = await templateinterface.DeleteTemplate(id);
                if (!responseData.IsSuccess)
                {
                    return Json(new { success = false, message = responseData.Message });
                }
                return Json(new { success = true, message = "Template deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error in adding documnet: " + ex.Message });
            }
        }


     

        //[HttpPost]
        // public ActionResult UploadImage(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        //{
        //if (upload.Length <= 0) return null;
        //if (!upload.IsImage())
        //{
        //    var NotImageMessage = "please choose a picture";
        //    dynamic NotImage = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + NotImageMessage + "\"}}");
        //    return Json(NotImage);
        //}

        //var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();

        //Image image = Image.FromStream(upload.OpenReadStream());
        //int width = image.Width;
        //int height = image.Height;
        //if ((width > 750) || (height > 500))
        //{
        //    var DimensionErrorMessage = "Custom Message for error"
        //    dynamic stuff = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + DimensionErrorMessage + "\"}}");
        //    return Json(stuff);
        //}

        //if (upload.Length > 500 * 1024)
        //{
        //    var LengthErrorMessage = "Custom Message for error";
        //    dynamic stuff = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + LengthErrorMessage + "\"}}");
        //    return Json(stuff);
        //}

        //var path = Path.Combine(
        //    Directory.GetCurrentDirectory(), "wwwroot/images/CKEditorImages",
        //    fileName);

        //using (var stream = new FileStream(path, FileMode.Create))
        //{
        //    upload.CopyTo(stream);

        //}

        //var url = $"{"/images/CKEditorImages/"}{fileName}";
        //var successMessage = "image is uploaded successfully";
        //dynamic success = JsonConvert.DeserializeObject("{ 'uploaded': 1,'fileName': \"" + fileName + "\",'url': \"" + url + "\", 'error': { 'message': \"" + successMessage + "\"}}");
        //return Json(success);
        //}
        //public static bool IsImage(this IFormFile file)
        //{
        //    try
        //    {
        //        var img = System.Drawing.Image.FromStream(file.OpenReadStream());
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}
