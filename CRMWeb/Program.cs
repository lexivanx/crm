using CRMWeb.Data;
using Microsoft.EntityFrameworkCore;
using CRMWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();

// Add authentication services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Login";
    });

// Add authorization services
builder.Services.AddAuthorization();

// Register AnalyticsService
builder.Services.AddScoped<AnalyticsService>();

// Register CRMContext with SQL Server
builder.Services.AddDbContext<CRMContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Redirect root URL to /Login if accessed
app.MapGet("/", async context =>
{
    context.Response.Redirect("/Login");
    await Task.CompletedTask;
});

// Run the app
app.Run();
