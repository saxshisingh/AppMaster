 
namespace PingaUnitBooking.UI.Middleware
{
    public class SessionExpirationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionExpirationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Session.IsAvailable || !context.Session.GetString("isLogin")?.Equals("true") == true)
            {
                // Redirect to logout action or perform logout logic
                context.Response.Redirect("/Index");
                return;
            }

            await _next(context);
        }
    }
}
