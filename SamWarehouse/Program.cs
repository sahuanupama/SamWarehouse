using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SamWarehouse.Models;
using SamWarehouse.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
//Adds the file uploader and encryption services to our services/dependency injection system.
builder.Services.AddScoped<FileUploaderService>();
builder.Services.AddScoped<EncryptionService>();

var connString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<ItemDbContext>(options =>
{
    options.UseSqlServer(connString);
});

//Adds sessions to the project and configures them to be used.
builder.Services.AddSession(options =>
{
    //Tells the session how long to go wihtout a request form the session user before
    //it clears the session data.
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    //Session cookie is only accessible through the HTTP protocol and not through
    //clinet side scripting such as javascript.
    options.Cookie.HttpOnly = true;
    //Sets whether the cookie can be used outside the domain where it was created.
    options.Cookie.SameSite = SameSiteMode.Strict;
    //Set whether the cookie m,ust be sent via https(always) or via the 
    //same methofd it was sent(Http or Https)
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});


//Adds the .NET framework authentication system to our project which can be configured
//to manage logins and access controls.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    //Sets the page to direct users to when they are being forced to login.
    options.LoginPath = "/Login/Login";
    //Sets how long the login lasts before it runs out
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    //Sets whether the expiry updates through user actions or not. The default
    //system for this cookie is to only update after half the expiry time has passed, rather
    //than on every action.
    options.SlidingExpiration = true;
    //Sets where to direct the user if they try to access a resource that is higher than their 
    //permission level.
    options.AccessDeniedPath = "/Login/AccessDenied";
});



// Add services to the container.
/*builder.Services.AddControllersWithViews(options =>
    {
        options.AddPolicy("Management", policy => policy.RequireRole("Admin", "Manager")
                                                        .RequireClaim("Department", "Management"));
    });*/

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
