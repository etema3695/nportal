namespace NewsPortal.Migrations
{
    using Microsoft.AspNetCore.Identity;
    using NewsPortal.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<NewsPortal.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(NewsPortal.Models.ApplicationDbContext context)
        {
            // Seed roles directly via EF6-backed ASP.NET Core Identity entities
            string[] roleNames = { "SuperAdmin", "Journalist" };
            foreach (var roleName in roleNames)
            {
                if (!context.Roles.Any(r => r.Name == roleName))
                {
                    context.Roles.Add(new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = roleName,
                        NormalizedName = roleName.ToUpperInvariant()
                    });
                }
            }
            context.SaveChanges();

            // Seed default SuperAdmin user
            const string adminEmail = "admin@newsportal.com";
            const string adminPassword = "Admin@123";

            if (!context.Users.Any(u => u.UserName == adminEmail))
            {
                var hasher = new PasswordHasher<ApplicationUser>();
                var admin = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = adminEmail,
                    NormalizedUserName = adminEmail.ToUpperInvariant(),
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpperInvariant(),
                    Phone = "0000000000",
                    PhoneNumber = "0000000000",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, adminPassword),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = true
                };

                context.Users.Add(admin);
                context.SaveChanges();

                var superAdminRole = context.Roles.First(r => r.Name == "SuperAdmin");
                context.Set<IdentityUserRole<string>>().Add(new IdentityUserRole<string>
                {
                    UserId = admin.Id,
                    RoleId = superAdminRole.Id
                });
                context.SaveChanges();
            }
        }
    }
}
