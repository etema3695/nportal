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
using System.Threading;
var builder = WebApplication.CreateBuilder(args);

// API controllers only (React frontend is separate)
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS — allow the React dev server (Vite default port)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

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

// Cookie authentication — return 401/403 for API instead of redirecting to views
builder.Services.ConfigureApplicationCookie(options =>
{
    options.SlidingExpiration = true;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return System.Threading.Tasks.Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return System.Threading.Tasks.Task.CompletedTask;
    };
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NewsPortal API v1"));
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
// Serve uploaded article images from /Images
var imagesPath = Path.Combine(app.Environment.ContentRootPath, "Images");
if (!Directory.Exists(imagesPath))
    Directory.CreateDirectory(imagesPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/Images"
});
app.UseRouting();

app.UseCors("ReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Migrate AspNetUsers schema from Identity v2 to Core Identity format
{
    using var db = new ApplicationDbContext(connectionString);
    db.Database.ExecuteSqlCommand(@"
        IF COL_LENGTH('AspNetUsers','NormalizedUserName') IS NULL
            ALTER TABLE AspNetUsers ADD NormalizedUserName NVARCHAR(256) NULL;
        IF COL_LENGTH('AspNetUsers','NormalizedEmail') IS NULL
            ALTER TABLE AspNetUsers ADD NormalizedEmail NVARCHAR(256) NULL;
        IF COL_LENGTH('AspNetUsers','ConcurrencyStamp') IS NULL
            ALTER TABLE AspNetUsers ADD ConcurrencyStamp NVARCHAR(MAX) NULL;
        IF COL_LENGTH('AspNetUsers','LockoutEnd') IS NULL
            ALTER TABLE AspNetUsers ADD LockoutEnd DATETIMEOFFSET NULL;
    ");
    // Populate normalized columns for existing users
    db.Database.ExecuteSqlCommand(@"
        UPDATE AspNetUsers
        SET NormalizedUserName = UPPER(UserName),
            NormalizedEmail = UPPER(Email),
            ConcurrencyStamp = NEWID()
        WHERE NormalizedUserName IS NULL;
    ");

    // Fix admin password hash to ASP.NET Core Identity v3 format
    var hasher  = new PasswordHasher<ApplicationUser>();
    var newHash = hasher.HashPassword(null, "Admin@123");
    var stamp   = Guid.NewGuid().ToString();
    int rows = db.Database.ExecuteSqlCommand(
        "UPDATE AspNetUsers SET PasswordHash = {0}, SecurityStamp = {1} WHERE UserName = {2}",
        newHash, stamp, "admin@newsportal.com");
    Console.WriteLine(rows > 0
        ? "✅ Admin password hash updated."
        : "⚠️  Admin user not found — register via /register.");
}

app.Run();

