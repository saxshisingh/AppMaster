using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingaUnitBooking.UI.Pages.Auth
{
    public class UserLoginModel : PageModel
    {
        public void OnGet()
        {
            if (Request.Cookies["isLogin"] == "true")
            {
            /*  Response.Redirect("../Dashboard/Dashboard");*/
            }
        }
    }
}
