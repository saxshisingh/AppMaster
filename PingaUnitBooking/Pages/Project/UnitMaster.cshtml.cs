using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingaUnitBooking.UI.Pages.Project
{
    public class UnitMasterModel : PageModel
    {
        public void OnGet()
        {
            if (HttpContext.Session.GetString("UnitDetail") != "True")
            {
                Response.Redirect("../Index");
            }
        }
    }
}
