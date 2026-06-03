using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using NewsPortal.Models;
using NewsPortal.Identity;
using NewsPortal.BLL.Services;
using NewsPortal.DAL.Repositories;
using NewsPOrtal.DAL.Repositories;
using NewsPOrtal.DAL.Models;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// MVC with Views
builder.Services.AddControllersWithViews();

// EF6 DbContext — scoped, reads connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'");
NewsPortalContext.ConnectionStringOverride = connectionString;
builder.Services.AddScoped<ApplicationDbContext>(_ => new ApplicationDbContext(connectionString));

// ASP.NET Core Identity with custom EF6-backed stores
builder.Services.AddScoped<IUserStore<ApplicationUser>, Ef6UserStore>();
builder.Services.AddScoped<IRoleStore<IdentityRole>, Ef6RoleStore>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddDefaultTokenProviders();

// Cookie authentication (replaces OWIN UseCookieAuthentication)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/LogOff";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Application services (DI)
builder.Services.AddScoped<ArticleService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<ArticleRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<CommentRepository>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(app.Environment.ContentRootPath),
    RequestPath = ""
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Default MVC route — matches /NewsPortal/index etc.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=NewsPortal}/{action=Index}/{id?}");

app.Run();

