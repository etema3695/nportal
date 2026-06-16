namespace NewsPortal.Migrations
{
    using Microsoft.AspNetCore.Identity;
    using NewsPortal.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<NewsPortal.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(NewsPortal.Models.ApplicationDbContext context)
        {
            // Seeding is handled in Program.cs startup code
        }
    }
}
