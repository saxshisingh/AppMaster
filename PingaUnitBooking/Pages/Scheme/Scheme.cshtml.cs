using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingaUnitBooking.UI.Pages.Scheme
{
    public class SchemeModel : PageModel
    {
        public void OnGet()
        {
            if (HttpContext.Session.GetString("Scheme") != "True")
            {
                Response.Redirect("../Index");
            }
        }
    }
}
