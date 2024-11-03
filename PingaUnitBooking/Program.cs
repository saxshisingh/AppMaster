using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Helpers;
using PingaUnitBooking.Infrastructure.Implementations;
using PingaUnitBooking.Infrastructure.Interfaces;
using PingaUnitBooking.UI.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);




// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddCors();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache(); // Add this line for session support

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//custom Services block
builder.Services.AddTransient<IAuthInterface, AuthService>();
builder.Services.AddSingleton<IDbInterface, DbService>();
builder.Services.AddTransient<IProjectInterface, ProjectService>();
builder.Services.AddTransient<IUnitInterface, UnitService>();
builder.Services.AddTransient<IBookingInterface, BookingUnitService>();
builder.Services.AddTransient<ITemplateInterface, TemplateService>();
builder.Services.AddTransient<IApplicationDocInterface, ApplicationDocService>();
builder.Services.AddSingleton<LocalStorageData>();

builder.Services.AddTransient<IDashboardInterface, DashboardService>();
builder.Services.AddTransient<IMailConfigureInterface, MailConfigureService>();
builder.Services.AddTransient<INotificationService ,Notification>();
builder.Services.AddTransient<ISchemeInterface, SchemeService>();
builder.Services.AddTransient<IReallocationInterface, ReallocationService>();
//end custom Services block


//Jwt configuration starts here 
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
     };
 });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors(options =>
        options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SessionExpirationMiddleware>();
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages(); //Routes for pages
    endpoints.MapControllers(); //Routes for my API controllers
});
app.Run();
