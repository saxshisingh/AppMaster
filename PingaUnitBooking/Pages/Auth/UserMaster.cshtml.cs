using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingaUnitBooking.UI.Pages.Auth
{
    public class UserMasterModel : PageModel
    {
        public void OnGet()
        {
            if (HttpContext.Session.GetString("RolePermission") != "True")
            {
                Response.Redirect("../Index");
            }
        }
    }
}
