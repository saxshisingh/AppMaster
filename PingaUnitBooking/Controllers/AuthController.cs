using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Helpers;
using PingaUnitBooking.Infrastructure.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
namespace PingaUnitBooking.UI.Controllers
{

    [Route("api/AuthController")]
    public class AuthController : Controller
    {
        private readonly IAuthInterface authIterface;
        IConfiguration _configuration;
        private readonly LocalStorageData _ld;
        public AuthController(IAuthInterface _authIterface, IConfiguration configuration, LocalStorageData ld)
        {
            authIterface = _authIterface;
            _configuration = configuration;
            _ld = ld;
        }
        [HttpPost]
        [Route("UserLogin")]
        public async Task<IActionResult> UserLogin([FromBody] AuthData auth)
        {
            try
            {
                var responseData = await authIterface.userLogin(auth);
                if (responseData.IsSuccess)
                {

                    bool isValid = new ValidateLicenseHelper().ValidateLicense(responseData.Data.Credential, responseData.Data.CredentialInfo, responseData.Data.ETADLLITEVITCA);
                    if (!isValid)
                    {
                        return Json(new { success = false, message = "License Expired" });
                    }
                    else
                    {
                        var accessToken = GenrateToken(responseData.Data.username, responseData.Data.userId, responseData.Data.groupID, responseData.Data.email, responseData.Data.roleName, responseData.Data.roleID, auth.rememberMe, auth.password);
                        var responseData2 = await authIterface.updateToken(responseData.Data.userId, accessToken, responseData.Data.groupID);
                        if (responseData2.IsSuccess)
                        {
                            await GetPermissions();
                            return Json(new { success = true, message = responseData.Message, Data = accessToken });
                        }
                        else
                        {
                            return Json(new { success = false, message = responseData.Message });
                        }
                    }

                 //   return Ok(responseData);
                }
                else
                {
                    return Json(new { success = false, message = responseData.Message });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Error in login process: " + ex.Message });
            }
        }



        /* [HttpGet]
         [Route("logOut")]
         public async Task<IActionResult> logOut()
         {
             try
             {

                 HttpContext.Session.Clear();

                 // Optionally, you can also clear the authentication cookie if you are using cookie authentication
                 // This will ensure the user is fully logged out
                 HttpContext.SignOutAsync();
                 HttpContext.Session.Clear();
                 return Ok();
             }
             catch (Exception ex)
             {
                 return Json(new { success = false, message = "Error : " + ex.Message });
             }
         }*/

        [HttpGet]
        [Route("logOut")]
        public IActionResult Logout()
        {

            try
            {
                // Clear the session
                HttpContext.Session.Clear();

                HttpContext.SignOutAsync();

                // Redirect to the login page 
                return Json(new { success = true });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Error :" + ex.Message });
            }

        }


        [HttpPost]
        [Route("addUser")]
        public async Task<IActionResult> addUser([FromBody] ubmUserData _ubmUserData)
        {
            try
            {
                dynamic responseData = null;
                foreach (var userId in _ubmUserData.useridList)
                {
                    var userData = new ubmUserData
                    {
                        userID = userId,
                        roleName = _ubmUserData.roleName,

                        CreatedBy = int.Parse(HttpContext.Session.GetString("userId")),
                        GroupID = decimal.Parse(HttpContext.Session.GetString("groupID")),


                    };
                    responseData = await authIterface.addUser(userData);
                }
                if (!responseData.IsSuccess)
                {
                    return Json(new { success = false, message = responseData.Message });
                }
                return Json(new { success = true, message = responseData.Message });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error in adding users: " + ex.Message });
            }
        }


        [HttpGet]
        // [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Route("userList")]
        public async Task<IActionResult> userList(string type)
        {
            try
            {
                var responseData = await authIterface.userList(decimal.Parse(HttpContext.Session.GetString("groupID")), int.Parse(HttpContext.Session.GetString("roleID")), type);
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
        [Route("changeStatus")]
        public async Task<IActionResult> changeStatus([FromQuery] int userID)
        {
            try
            {
                var responseData = await authIterface.changeStatus(userID, decimal.Parse(HttpContext.Session.GetString("groupID")));
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

        public string GenrateToken(string username, decimal userId, decimal groupID, string email, string roleName, decimal roleID,bool rememberMe, string password)
        {

            var accessToken = "";
            LocalStorageData ld = new LocalStorageData();

            //SET THE LOCAL STORAGE VALUE

            HttpContext.Session.SetString("username", username);
            HttpContext.Session.SetString("userId", userId.ToString());
            HttpContext.Session.SetString("groupID", groupID.ToString());
            HttpContext.Session.SetString("email", email);
            HttpContext.Session.SetString("roleName", roleName);
            HttpContext.Session.SetString("roleID", roleID.ToString());
            HttpContext.Session.SetString("isLogin", "true");

            // GET THE LOCAL STORAGE VALUE IN LOCAL STORAGE CLASS

            //SET THE REMEMBERME VALUE IN COOKIES 

            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddDays(20);
            Response.Cookies.Append("rememberMe", rememberMe.ToString(), options);
            if (Request.Cookies["rememberMe"] == "true")
            {
                Response.Cookies.Append("email", email.ToString(), options);
                Response.Cookies.Append("password", password.ToString(), options);
            }
            /* _ld.Username = HttpContext.Session.GetString("username");
             _ld.UserID = int.Parse(HttpContext.Session.GetString("userId"));
             decimal.Parse(HttpContext.Session.GetString("groupID")) = decimal.Parse(HttpContext.Session.GetString("groupID"));
             _ld.Email = HttpContext.Session.GetString("email");
             _ld.RoleName = HttpContext.Session.GetString("roleName");
             _ld.RoleID = int.Parse(HttpContext.Session.GetString("roleID"));
             _ld.IsLogin = bool.Parse(HttpContext.Session.GetString("isLogin"));*/





            //SET THE VALUES IN CLAIMS

            var claims = new[] {
                                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),

                                 new Claim("UserName", username.ToString()),
                                 new Claim("userId", userId.ToString()),
                                 new Claim("groupID", groupID.ToString()),
                                 new Claim("email", email.ToString()),
                                 new Claim("roleName", roleName.ToString()),
                                 new Claim("roleID", roleID.ToString()),
                            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(10),
                signingCredentials: signIn);
            accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }



        [HttpPost]
        [Route("customerAuth")]
        public async Task<IActionResult> customerAuth([FromBody] AuthData auth)
        {
            try
            {
                var responseData = await authIterface.customerAuth(auth);
                if (responseData.IsSuccess)
                {
                    foreach (var item in responseData.Data)
                    {

                        //SET THE LOCAL STORAGE VALUE
                        HttpContext.Session.SetString("userId", item.userId.ToString());
                        HttpContext.Session.SetString("groupID", item.groupID.ToString());

                        // GET THE LOCAL STORAGE VALUE IN LOCAL STORAGE CLASS
                        /*
                                                _ld.UserID = int.Parse(HttpContext.Session.GetString("userId"));
                                                decimal.Parse(HttpContext.Session.GetString("groupID")) = decimal.Parse(HttpContext.Session.GetString("groupID"));*/
                    }
                    return Ok(responseData);
                }
                else
                {
                    return Json(new { success = false, message = responseData.Message });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Error in login process: " + ex.Message });
            }
        }






        /*
                [HttpGet("get-session-data")]
                public IActionResult GetSessionData()
                {
                    var sessionData = new
                    {
                        _ld.Username,
                        _ld.UserID,
                        decimal.Parse(HttpContext.Session.GetString("groupID")),
                        _ld.Email,
                        _ld.RoleName,
                        _ld.RoleID,
                        _ld.IsLogin
                    };

                    return Ok(sessionData);
                }*/



        [HttpGet]
        [Route("GetPermissions")]
        public async Task<IActionResult> GetPermissions(string pageType = null)
        {
            try
            {
                var responseData = await authIterface.GetPermissions(int.Parse(HttpContext.Session.GetString("userId")), decimal.Parse(HttpContext.Session.GetString("groupID")), pageType);
                if (responseData.IsSuccess)
                {
                    if (pageType == null)
                    {
                        RoleMaster _rm = new RoleMaster();
                        //SET THE LOCAL STORAGE VALUE

                        HttpContext.Session.SetString("Dashboard", responseData.Data[0].isTab.ToString());
                        HttpContext.Session.SetString("RolePermission", responseData.Data[1].isTab.ToString());
                        HttpContext.Session.SetString("ProjectPermission", responseData.Data[2].isTab.ToString());
                        HttpContext.Session.SetString("UnitDetail", responseData.Data[3].isTab.ToString());
                        HttpContext.Session.SetString("Template", responseData.Data[4].isTab.ToString());
                        HttpContext.Session.SetString("Documents", responseData.Data[5].isTab.ToString());
                        HttpContext.Session.SetString("UnitBooking", responseData.Data[6].isTab.ToString());
                        HttpContext.Session.SetString("UnitBooking", responseData.Data[6].isTab.ToString());
                        HttpContext.Session.SetString("Scheme", responseData.Data[7].isTab.ToString());
                        HttpContext.Session.SetString("BookingReallocation", responseData.Data[8].isTab.ToString());
                    }

                    return Json(new { success = true, data = responseData.Data });
                }
                else
                {
                    return Json(new { success = false, message = responseData.Message });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Error in login process: " + ex.Message });
            }
        }


    }
}
