using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace PingaUnitBooking.UI.Pages.Dashboard
{
    public class DashboardModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Dashboard") != "True")
            {
                return RedirectToPage("../Index");
            }
            ViewData["RoleName"] = HttpContext.Session.GetString("roleName");
            return Page();
        }
    }
}
