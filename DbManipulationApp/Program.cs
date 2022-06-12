using DbManipulationApp.Data;
using DbManipulationApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString_identity = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");

builder.Services.AddDbContext<DbManipulationAppContext>(options =>
    options.UseSqlServer(connectionString_identity));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DbManipulationAppContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString_czytania = builder.Configuration.GetConnectionString("CzytaniaConnection") ?? throw new InvalidOperationException("Connection string 'CzytaniaConnection' not found ");
builder.Services.AddDbContext<czytaniaContext>(options => options.UseSqlServer(connectionString_czytania));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.ConfigureApplicationCookie(options =>
{
    //Location for your Custom Access Denied Page
    options.AccessDeniedPath = "/Home/Index";

    //Location for your Custom Login Page
    options.LoginPath = "/Home/Index";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
