using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingaUnitBooking.UI.Pages.Configuration
{
    public class TemplateConfigurationModel : PageModel
    {
        public void OnGet()
        {
            if (HttpContext.Session.GetString("Template") != "True")
            {
                Response.Redirect("../Index");
            }
            
        }
    }
}
