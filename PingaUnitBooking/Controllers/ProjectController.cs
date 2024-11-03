using Microsoft.AspNetCore.Mvc;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;


namespace PingaUnitBooking.UI.Controllers
{
    [Route("api/projectController")]
    public class ProjectController : Controller
    {
        private readonly IProjectInterface _projectInterface;
        private readonly LocalStorageData _ld;

        public ProjectController(IProjectInterface projectInterface , LocalStorageData _ld)
        {
         this._projectInterface = projectInterface;
         this._ld = _ld;
        }

        [HttpPost]
        [Route("SearchData")]
        public async Task<IActionResult> SearchData([FromBody] SearchData searchData)
        {
            try
            {
               
                List<decimal> companyList = new List<decimal>();
                List<decimal> locationList = new List<decimal>();
                List<decimal> projectList = new List<decimal>();
                var groupID = decimal.Parse(HttpContext.Session.GetString("groupID"));

                if (searchData == null || string.IsNullOrEmpty(searchData.type))
                {
                    return Json(new { success = false, message = "Invalid search data." });
                }

                switch (searchData.type)
                {
                    case "Company":
                        searchData.search ="GroupID="+ groupID;
                        break;

                    case "Location":
                        if (searchData.projectAllData != null)
                        {
                            foreach (var companyData in searchData.projectAllData.company)
                            {
                                companyList.Add(companyData.companyID);
                            }
                            searchData.search = $"groupId = {groupID} and companyid in ({string.Join(",", companyList)})";
                        }
                        break;

                    case "Project":
                        if (searchData.projectAllData != null)
                        {
                            foreach (var companyData in searchData.projectAllData.company)
                            {
                                companyList.Add(companyData.companyID);
                            }
                            foreach (var locationData in searchData.projectAllData.location)
                            {
                                locationList.Add(locationData.locationID);
                            }
                            searchData.search = $"groupId = {groupID} and companyid in ({string.Join(",", companyList)}) and locationid in ({string.Join(",", locationList)})";
                        }
                        break;
                    case "Tower":
                        if (searchData.projectAllData != null)
                        {
                            foreach (var companyData in searchData.projectAllData.company)
                            {
                                companyList.Add(companyData.companyID);
                            }
                            foreach (var locationData in searchData.projectAllData.location)
                            {
                                locationList.Add(locationData.locationID);
                            }
                            foreach (var projectData in searchData.projectAllData.project)
                            {
                                projectList.Add(projectData.projectID);
                            }
                            searchData.search = $"mp.groupId = {groupID} and mu.companyid in ({string.Join(",", companyList)}) and mu.locationid in ({string.Join(",", locationList)}) and mu.projectid in ({string.Join(",", projectList)}) and mu.parentID is null";
                        }

                        break;

                    default:
                        return Json(new { success = false, message = "Unknown search type." });
                }
                var responseData = await _projectInterface.searchData(searchData);

                if (responseData.IsSuccess)

                {
                  /*  var companyListObj = new List<object>();
                    if (searchData.type == "Tower")
                    {
                        if (responseData.Data != null)
                        {
                            // Ensure responseData.Data is an enumerable collection
                            foreach (var item in responseData.Data)
                            {
                                if (item.tower != null)
                                {
                                    companyListObj.AddRange(item.tower.Select(c => new { c.towerID, c.towerName }));
                                }
                            }
                        }
                        return Json(new { success = true, data = responseData.Data, companyListObj });
                    }*/
                    return Json(new { success = true, data = responseData.Data });
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
   
        [HttpPost]
        [Route("addProjectPermission")]
        public async Task<IActionResult> addProjectPermission([FromBody] BindProjectPermissionDTO _bindProjectPermissionDTO)
        {
            try
            {
                dynamic responseData = null;
                foreach (var userId in _bindProjectPermissionDTO.useridList)
                {
                    if (_bindProjectPermissionDTO.towerList.Count > 0)
                    {
                        foreach (var towerId in _bindProjectPermissionDTO.towerList)
                        {
                            var userData = new BindProjectPermissionDTO
                            {
                                userID = userId,
                                companyid = _bindProjectPermissionDTO.companyid,
                                locationid = _bindProjectPermissionDTO.locationid,
                                projectid = _bindProjectPermissionDTO.projectid,
                                towerID = towerId,
                                CreatedBy = int.Parse(HttpContext.Session.GetString("userId")),
                                GroupID = decimal.Parse(HttpContext.Session.GetString("groupID")),
                            };
                            responseData = await _projectInterface.addProjectPermission(userData);
                            if (!responseData.IsSuccess)
                            {
                                return Json(new { success = false, message = responseData.Message });
                            }
                        }

                    }
                    else
                    {


                        
                        var userData = new BindProjectPermissionDTO
                        {
                            userID = userId,
                            companyid = _bindProjectPermissionDTO.companyid,
                            locationid = _bindProjectPermissionDTO.locationid,
                            projectid = _bindProjectPermissionDTO.projectid,
                            CreatedBy = int.Parse(HttpContext.Session.GetString("userId")),
                            GroupID = decimal.Parse(HttpContext.Session.GetString("groupID")),
                        };
                        responseData = await _projectInterface.addProjectPermission(userData);
                        if (!responseData.IsSuccess)
                        {
                            return Json(new { success = false, message = responseData.Message });
                        }
                    }



                }
                return Json(new { success = responseData.IsSuccess, message = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error in adding users: " + ex.Message });
            }
        }


        [HttpGet]
        // [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Route("projectPermissionlist")]
        public async Task<IActionResult> projectPermissionlist(int userID)
        {
            try
            {
                
                var responseData = await _projectInterface.projectPermissionlist(decimal.Parse(HttpContext.Session.GetString("groupID")),userID);
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
        [Route("deletePermission")]
        public async Task<IActionResult> deletePermission([FromQuery] int permissionID)
        {
            try
            {
                var responseData = await _projectInterface.deletePermission(permissionID, decimal.Parse(HttpContext.Session.GetString("groupID")));
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
                return Json(new { success = false, message = ex.Message });
            }
        }



    }

}
