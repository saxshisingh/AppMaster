using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingaUnitBooking.UI.Pages.Application
{
    public class ApplicationMasterModel : PageModel
    {
        public void OnGet()
        {
            if (HttpContext.Session.GetString("UnitBooking") != "True")
            {
                Response.Redirect("../Index");
            }
        }
    }
}
