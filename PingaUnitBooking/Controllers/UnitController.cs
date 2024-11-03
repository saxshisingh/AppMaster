using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;

namespace PingaUnitBooking.UI.Controllers
{
    [Route("api/UnitController")]
    public class UnitController : Controller
    {

        private readonly IUnitInterface _unitInterface;
        private readonly LocalStorageData _ld;
        public UnitController(IUnitInterface unitInterface, LocalStorageData _ld)
        {
            this._unitInterface = unitInterface;
            this._ld = _ld;
        }

        [HttpGet]
        [Route("unitDetailsList")]
        public async Task<IActionResult> unitDetailsList([FromQuery] int ProjectID, int TowerID, string statusType)
        {
            try
            {
                var responseData = await _unitInterface.unitDetailsList(decimal.Parse(HttpContext.Session.GetString("groupID")), int.Parse(HttpContext.Session.GetString("userId")), ProjectID, TowerID, statusType);
                if (responseData.IsSuccess)
                {
                    foreach (var item in responseData.Data)
                    {
                        item.roleName = HttpContext.Session.GetString("roleName");
                    }

                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, data = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("paymentPlanList")]
        public async Task<IActionResult> paymentPlanList(decimal blockID, decimal unitID, decimal companyID, decimal locationID)
        {
            try
            {
                var responseData = await _unitInterface.paymentPlanList(blockID, unitID, companyID, locationID);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, data = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("intrestPlanList")]
        public async Task<IActionResult> intrestPlanList(decimal companyID, decimal locationID)
        {
            try
            {

                var responseData = await _unitInterface.intrestPlanList(companyID, locationID, decimal.Parse(HttpContext.Session.GetString("groupID")));
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, data = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }


        [HttpGet]
        [Route("changeUnitStatus")]
        public async Task<IActionResult> changeUnitStatus([FromQuery] int unitID, int status)
        {
            try
            {
                var responseData = await _unitInterface.changeUnitStatus(unitID, decimal.Parse(HttpContext.Session.GetString("groupID")), status);
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
        [HttpPost]
        [Route("addUbmUnit")]
        public async Task<IActionResult> addUbmUnit([FromBody] UnitData _unitData)
        {
            try
            {
                if (_unitData == null)
                    throw new Exception("Issue in unit configuration!");
                _unitData.userID = int.Parse(HttpContext.Session.GetString("userId"));
                _unitData.groupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var responseData = await _unitInterface.addUbmUnit(_unitData);
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

        [HttpGet]
        [Route("GetUserProjects")]
        public async Task<IActionResult> GetUserProjects()
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var ubmUserId = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await _unitInterface.GetUserProjects(GroupID, ubmUserId);
                if (responseData.IsSuccess)
                {
                    return Json(new { success = true, data = responseData.Data });
                }
                return Json(new { success = false, data = responseData.Message });

            }
            catch (Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetTowerByProjectId")]
        public async Task<IActionResult> GetTowerByProjectId([FromQuery] int ProjectId)
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var ubmUserId = int.Parse(HttpContext.Session.GetString("userId"));
                var responseData = await _unitInterface.GetTowerByProjectId(GroupID, ubmUserId, ProjectId);
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
        [Route("GetPaymentPlan")]
        public async Task<IActionResult> GetPaymentPlan([FromQuery] int PayPlanID)
        {
            try
            { 
                var responseData = await _unitInterface.GetPaymentPlan(PayPlanID);
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
        [Route("GetScheme")]
        public async Task<IActionResult> GetScheme()
        {
            try
            {
                var GroupID = decimal.Parse(HttpContext.Session.GetString("groupID"));
                var responseData = await _unitInterface.GetScheme(GroupID);
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
