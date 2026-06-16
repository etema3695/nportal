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

// Serve the React SPA from wwwroot (production Docker build)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("ReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SPA fallback: serve index.html for non-API routes when running in Docker
app.MapFallbackToFile("index.html");

// Migrate AspNetUsers schema from Identity v2 to Core Identity format
{
    bool success = false;
    int retries = 0;
    const int maxRetries = 10;
    const int delayMs = 5000;

    while (!success && retries < maxRetries)
    {
        try
        {
            using var db = new ApplicationDbContext(connectionString);

            // Override the null initializer set in the constructor — run migrations
            System.Data.Entity.Database.SetInitializer(
                new System.Data.Entity.MigrateDatabaseToLatestVersion<ApplicationDbContext, NewsPortal.Migrations.Configuration>(true));
            db.Database.Initialize(force: true);

            // Ensure Phone column exists (not covered by code-based migrations)
            db.Database.ExecuteSqlCommand(@"
                IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
                BEGIN
                    IF COL_LENGTH('AspNetUsers','NormalizedUserName') IS NULL
                        ALTER TABLE AspNetUsers ADD NormalizedUserName NVARCHAR(256) NULL;
                    IF COL_LENGTH('AspNetUsers','NormalizedEmail') IS NULL
                        ALTER TABLE AspNetUsers ADD NormalizedEmail NVARCHAR(256) NULL;
                    IF COL_LENGTH('AspNetUsers','ConcurrencyStamp') IS NULL
                        ALTER TABLE AspNetUsers ADD ConcurrencyStamp NVARCHAR(MAX) NULL;
                    IF COL_LENGTH('AspNetUsers','LockoutEnd') IS NULL
                        ALTER TABLE AspNetUsers ADD LockoutEnd DATETIMEOFFSET NULL;
                    IF COL_LENGTH('AspNetUsers','Phone') IS NULL
                        ALTER TABLE AspNetUsers ADD Phone NVARCHAR(50) NULL;
                END
            ");
            // Populate normalized columns for existing users
            db.Database.ExecuteSqlCommand(@"
                IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
                BEGIN
                    UPDATE AspNetUsers
                    SET NormalizedUserName = UPPER(UserName),
                        NormalizedEmail = UPPER(Email),
                        ConcurrencyStamp = NEWID()
                    WHERE NormalizedUserName IS NULL;
                END
            ");

            // Fix admin password hash to ASP.NET Core Identity v3 format
            var hasher  = new PasswordHasher<ApplicationUser>();
            var newHash = hasher.HashPassword(null, "Admin@123");
            var stamp   = Guid.NewGuid().ToString();
            int rows = db.Database.ExecuteSqlCommand(
                @"IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
                    UPDATE AspNetUsers SET PasswordHash = {0}, SecurityStamp = {1} WHERE UserName = {2}",
                newHash, stamp, "admin@newsportal.com");
            Console.WriteLine(rows > 0
                ? "✅ Admin password hash updated."
                : "⚠️  Admin user not found — register via /register.");

            // Seed roles and assign admin to SuperAdmin
            db.Database.ExecuteSqlCommand(@"
                IF OBJECT_ID('AspNetRoles', 'U') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'SuperAdmin')
                        INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES (NEWID(), 'SuperAdmin', 'SUPERADMIN');
                    IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Journalist')
                        INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES (NEWID(), 'Journalist', 'JOURNALIST');
                END
            ");
            db.Database.ExecuteSqlCommand(@"
                IF OBJECT_ID('AspNetUserRoles', 'U') IS NOT NULL
                AND OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
                AND OBJECT_ID('AspNetRoles', 'U') IS NOT NULL
                BEGIN
                    DECLARE @uid NVARCHAR(128) = (SELECT Id FROM AspNetUsers WHERE UserName = 'admin@newsportal.com');
                    DECLARE @rid NVARCHAR(128) = (SELECT Id FROM AspNetRoles WHERE Name = 'SuperAdmin');
                    IF @uid IS NOT NULL AND @rid IS NOT NULL
                        AND NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @uid AND RoleId = @rid)
                        INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@uid, @rid);
                END
            ");
            success = true;
        }
        catch (Exception ex)
        {
            retries++;
            Console.WriteLine($"⚠️  Database initialization failed (attempt {retries}/{maxRetries}): {ex.Message}");
            if (retries < maxRetries)
            {
                Console.WriteLine($"   Retrying in {delayMs}ms...");
                Thread.Sleep(delayMs);
            }
            else
            {
                Console.WriteLine("❌ Database initialization failed after max retries. Starting anyway...");
            }
        }
    }
}

app.Run();
