using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingaUnitBooking.UI.Pages.Project
{
    public class ReallocationModel : PageModel
    {
        public void OnGet()
        {
            if (HttpContext.Session.GetString("BookingReallocation") != "True")
            {
                Response.Redirect("../Index");
            }
        }
    }
}
