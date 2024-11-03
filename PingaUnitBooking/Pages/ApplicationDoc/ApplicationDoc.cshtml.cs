using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingaUnitBooking.UI.Pages.ApplicationDoc
{
    public class ApplicationDocModel : PageModel
    {
        public void OnGet()
        {
            if (HttpContext.Session.GetString("Documents") != "True")
            {
                Response.Redirect("../Index");
            }
        }
    }
}
