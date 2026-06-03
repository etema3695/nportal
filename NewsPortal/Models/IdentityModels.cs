using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using Microsoft.AspNetCore.Identity;
using NewsPOrtal.DAL.Models;

namespace NewsPortal.Models
{
    /// <summary>
    /// Application user — backed by the existing AspNetUsers table via EF6.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Please enter phone number")]
        [StringLength(50)]
        public string Phone { get; set; }
    }

    /// <summary>
    /// EF6 DbContext covering both Identity tables and application tables.
    /// </summary>
    public class ApplicationDbContext : System.Data.Entity.DbContext
    {
        // Application tables
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }

        // Identity tables (mapped to existing AspNet* schema)
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<IdentityUserRole<string>> UserRoles { get; set; }

        // EF6 parameterless constructor — used by migrations and legacy code
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        // Constructor accepting a connection string — used by ASP.NET Core DI
        public ApplicationDbContext(string connectionString)
            : base(connectionString)
        {
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map Identity types to existing ASP.NET Identity tables
            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles")
                .HasKey(r => new { r.UserId, r.RoleId });
        }
    }
}


