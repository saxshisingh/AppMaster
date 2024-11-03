using MailKit.Search;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Helpers;
using PingaUnitBooking.Infrastructure.Interfaces;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PingaUnitBooking.UI.Controllers
{
    [Route("api/bookingController")]
    public class BookingUnitController : Controller
    {
        private readonly IBookingInterface bookingInterface;
        private readonly INotificationService _notificationService;
        private readonly LocalStorageData _ld;
        private IWebHostEnvironment Environment;
        public BookingUnitController(IBookingInterface bookingInterface, INotificationService notificationService, LocalStorageData _localStorage, IWebHostEnvironment webHost)
        {
            this.bookingInterface = bookingInterface;
            this._ld = _localStorage;
            this.Environment = webHost;
            _notificationService = notificationService;
        }
        [HttpGet]
        // [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Route("bookingUnitList")]
        public async Task<IActionResult> bookingUnitList(string SearchType, string SearchText)
        {
            try
            {
                var responseData = await bookingInterface.bookingUnitList(SearchType, SearchText,decimal.Parse(HttpContext.Session.GetString("groupID")), int.Parse(HttpContext.Session.GetString("userId")));
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, message = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }

        [HttpGet]
        // [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Route("ubmDetailsByUnitID")]
        public async Task<IActionResult> ubmDetailsByUnitID([FromQuery] int ubmID)
        {
            try
            {
                var responseData = await bookingInterface.ubmDetailsByUnitID(decimal.Parse(HttpContext.Session.GetString("groupID")), ubmID);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, message = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("getProjectDataforBooking")]
        public async Task<IActionResult> getProjectDataforBooking([FromBody] SearchData searchData)
        {
            try
            {
                if (searchData != null)
                {
                    searchData.groupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                    searchData.userID = int.Parse(HttpContext.Session.GetString("userId"));
                    var responseData = await bookingInterface.getProjectDataforBooking(searchData);

                    if (responseData.IsSuccess)

                    {
                        return Json(new { success = true, data = responseData.Data });
                        //return Json(new { success = true, data = responseData.Data });
                    }
                    else
                    {
                        return Json(new { success = false, message = responseData.Message });
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("addBookedUnit")]
        public async Task<IActionResult> addBookedUnit([FromBody] BookingData _bookingData)
        {
            try
            {

                _bookingData.groupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                _bookingData.createdBy = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await bookingInterface.addBookedUnit(_bookingData);

                if (responseData.IsSuccess)
                {
                    
                        if (_bookingData.UnitType.ToUpper() == "UNITBOOKING" && _bookingData.ubmID == null)
                            await _notificationService.SendNotifiction(_bookingData.groupID, _bookingData.createdBy, Convert.ToInt32(responseData.Data.ubmID), "Initiate Booking");
                    
                        return Json(new { success = true, message = responseData.Message, data = responseData.Data });
                    //return Json(new { success = true, data = responseData.Data });
                }
                else
                {
                    return Json(new { success = false, message = responseData.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        [Route("sendInitiateBookingMail")]
        public async Task<IActionResult> sendInitiateBookingMail(int ubmID)
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var UserID = int.Parse(HttpContext.Session.GetString("userId"));
                await _notificationService.SendNotifiction(GroupID, UserID, ubmID, "Initiate Booking");
                return Json(new { success = true, message = "Mail sent successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("addApplicantDetails")]
        public async Task<IActionResult> addApplicantDetails([FromBody] ApplicantData _applicantData)
        {
            try
            {
                _applicantData.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                _applicantData.CreatedBy = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await bookingInterface.addApplicantDetails(_applicantData);

                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, message = responseData.Message, });
                    //return Json(new { success = true, data = responseData.Data });
                }
                else
                {
                    return Json(new { success = false, message = responseData.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        [Route("getApplicantList")]
        public async Task<IActionResult> getApplicantList([FromQuery] int ubmID, int appType)
        {
            try
            {

                var responseData = await bookingInterface.getApplicantList(decimal.Parse(HttpContext.Session.GetString("groupID")), ubmID, appType);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, message = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }




        [HttpPost]
        [Route("addPaymentModel")]
        public async Task<IActionResult> addPaymentModel([FromBody] PaymentModel _paymentModel)
        {
            try
            {

                _paymentModel.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                _paymentModel.CreatedBy = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await bookingInterface.addPaymentModel(_paymentModel);

                if (responseData.IsSuccess)

                {
                    return Json(new { success = true, message = responseData.Message, data = responseData.Data });
                    //return Json(new { success = true, data = responseData.Data });
                }
                else
                {
                    return Json(new { success = false, message = responseData.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        [Route("getPaymentModelList")]
        public async Task<IActionResult> getPaymentModelList([FromQuery] int ubmID)
        {
            try
            {

                var responseData = await bookingInterface.getPaymentModelList(decimal.Parse(HttpContext.Session.GetString("groupID")), ubmID);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, message = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }



        [HttpGet]
        [Route("getApplicantDocument")]
        public async Task<IActionResult> getApplicantDocument([FromQuery] int ubmID)
        {
            try
            {

                var responseData = await bookingInterface.getApplicantDocument(decimal.Parse(HttpContext.Session.GetString("groupID")), ubmID);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, message = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }

     
                [HttpPost]
                [Route("addApplicantDocument")]
                public async Task<IActionResult> addApplicantDocument([FromForm] ApplicationDoc _doc)
                {
                    try
                    {

                        foreach (var attachment in _doc.DocumentFile)
                        {
                            //ADDING IMAGE PATH IN FOLDER 
                            var uniqueName = _doc.unitID.ToString() + "_" + _doc.MobileNo.ToString() + "_" + _doc.DocumentName.ToString();
                            //var uniqueName = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
                            string uploadsFolder = Path.Combine("Attachments", decimal.Parse(HttpContext.Session.GetString("groupID")).ToString(), _doc.unitID.ToString() + "_" + _doc.MobileNo.ToString());
                            if (!Directory.Exists(uploadsFolder))
                            {
                                System.IO.Directory.CreateDirectory(Path.Combine(this.Environment.WebRootPath, uploadsFolder));
                            }
                            var exten = System.IO.Path.GetExtension((attachment.FileName));
                            var newFileName = uniqueName + exten;
                            string filePath = Path.Combine(uploadsFolder, newFileName);
                            var filepp = filePath;
                            filePath = Path.Combine(this.Environment.WebRootPath, filePath);
                            bool add = false;
                            var fileName1 = attachment.FileName;
                            while (add == false)
                            {

                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath);
                                }
                                else
                                {
                                    add = true;
                                }
                            }
                            filePath = Path.Combine(this.Environment.WebRootPath, filePath);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                attachment.CopyTo(fileStream);
                            }
                            _doc.DocumentUrl = Path.Combine(uploadsFolder, newFileName);
                        }
                        _doc.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                        var responseData = await bookingInterface.addApplicantDocuments(_doc);
                        if (responseData.IsSuccess)
                        {
                            return Json(new { success = true, message = responseData.Message });
                        }
                        return Json(new { success = false, message = responseData.Message });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { succes = false, message = ex.Message });
                    }
                }
       

       /* [HttpPost]
        [Route("addApplicantDocument")]
        public async Task<IActionResult> AddApplicantDocument([FromForm] ApplicationDoc _doc)
        {
            try
            {
                foreach (var attachment in _doc.DocumentFile)
                {
                   
                    string baseFolder = @"D:\Uploads"; // Change this to your desired base folder
                    var uniqueName = $"{_doc.unitID}_{_doc.MobileNo}_{_doc.DocumentName}";
                    var exten = Path.GetExtension(attachment.FileName);
                    var newFileName = uniqueName + exten;

                    // Construct the full path for the file
                    string uploadsFolder = Path.Combine(baseFolder, $"{_doc.unitID}_{_doc.MobileNo}");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string filePath = Path.Combine(uploadsFolder, newFileName);
                    bool add = false;

                    while (add == false)
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        else
                        {
                            add = true;
                        }
                    }

                    // Save the file to the custom location
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachment.CopyToAsync(fileStream);
                    }

                    // Store the document URL (path relative to the base folder)
                    _doc.DocumentUrl = Path.Combine($"{_doc.unitID}_{_doc.MobileNo}", newFileName);
                }

                _doc.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var responseData = await bookingInterface.addApplicantDocuments(_doc);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, message = responseData.Message });
                }
                return Json(new { success = false, message = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
*/

        [HttpDelete]
        [Route("deleteAttachments")]
        public async Task<IActionResult> deleteAttachments([FromBody] ApplicationDoc _doc)
        {
            try
            {
                if (_doc.DocumentUrl != null)
                {
                    var FilePath = Path.Combine(this.Environment.WebRootPath, _doc.DocumentUrl);
                    System.IO.File.Delete(FilePath);
                    _doc.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                    var responseData = await bookingInterface.DeleteAttachments(_doc);
                    if (responseData.IsSuccess)
                    {
                        return Json(new { success = true, message = responseData.Message });
                    }
                    else
                    {
                        return Json(new { success = false, message = responseData.Message });
                    }

                }
                else
                {
                    return Json(new { success = false, message = "Document Url is Missing" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("deleteCoApplicant")]
        public async Task<IActionResult> deleteCoApplicant([FromBody] ApplicantData _applicantData)
        {
            try
            {
                _applicantData.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var responseData = await bookingInterface.deleteCoApplicant(_applicantData);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, message = responseData.Message });
                }
                else
                {
                    return Json(new { success = false, message = responseData.Message });
                }


            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("ChangeUbmStatus")]
        public async Task<IActionResult> ChangeUbmStatus([FromBody] UbmStatus _ubmStatus)
        {
            try
            {
                _ubmStatus.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                _ubmStatus.CreatedBy = int.Parse(HttpContext.Session.GetString("userId"));

                var responseData = await bookingInterface.ChangeUbmStatus(_ubmStatus);
                if (responseData.IsSuccess)
                {
                    if (HttpContext.Session.GetString("roleName").ToUpper()  == "ADMIN/CFO")
                        await _notificationService.SendNotifiction(_ubmStatus.GroupID, _ubmStatus.CreatedBy, Convert.ToInt32(_ubmStatus.UbmID), "Final Booking");
                    return Json(new { success = true, message = responseData.Message });
                }
                else
                {
                    return Json(new { success = false, message = responseData.Message });
                }


            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("ChangeUbmAuthorization")]
        public async Task<IActionResult> ChangeUbmAuthorization([FromBody] UbmStatus _ubmStatus)
        {
            try
            {
                _ubmStatus.GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                _ubmStatus.CreatedBy = int.Parse(HttpContext.Session.GetString("userId"));

                var responseData = await bookingInterface.ChangeUbmAuthorization(_ubmStatus);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, message = responseData.Message });
                }
                else
                {
                    return Json(new { success = false, message = responseData.Message });
                }


            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetApplicationDocument")]
        public async Task<IActionResult> GetApplicationDocument([FromQuery] string ApplicationType)
        {
            try
            {

                var responseData = await bookingInterface.GetApplicationDocument(decimal.Parse(HttpContext.Session.GetString("groupID")), ApplicationType);
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

        [HttpDelete]
        [Route("deletepaymentPlan")]
        public async Task<IActionResult> deletepaymentPlan([FromQuery] int paymentID)
        {
            try
            {
                var responseData = await bookingInterface.DeletePaymentPlan(decimal.Parse(HttpContext.Session.GetString("groupID")), paymentID);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, message = responseData.Message });
                }
                else
                {
                    return Json(new { success = false, message = responseData.Message });
                }


            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }


        [HttpGet]
        [Route("getTncTemplate")]
        public async Task<IActionResult> getTncTemplate([FromQuery] int ubmID)
        {
            try
            {
                var responseData = await bookingInterface.getTncTemplate(decimal.Parse(HttpContext.Session.GetString("groupID")), ubmID);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                else
                {
                    return Json(new { success = false, message = responseData.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetUnitInfo")]
        public async Task<IActionResult> GetUnitInfo([FromQuery] int UnitID)
        {
            try
            {


                var responseData = await bookingInterface.GetUnitInfo(decimal.Parse(HttpContext.Session.GetString("groupID")), UnitID);
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
        [Route("unitLogs")]
        public async Task<IActionResult> unitLogs([FromQuery] int ubmID)
        {
            try
            {
                var responseData = await bookingInterface.GetUnitLogs(decimal.Parse(HttpContext.Session.GetString("groupID")), ubmID);
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
        [Route("GetBrokerList")]
        public async Task<IActionResult> GetBrokerList()
        {
            try
            {
                var responseData = await bookingInterface.GetBrokerList(decimal.Parse(HttpContext.Session.GetString("groupID")));
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



    }
}
