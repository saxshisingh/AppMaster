using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingaUnitBooking.UI.Pages.Project
{
    public class ProjectMasterModel : PageModel
    {
        public void OnGet()
        {
            if (HttpContext.Session.GetString("ProjectPermission") != "True")
            {
                Response.Redirect("../Index");
            }
        }
    }
}
