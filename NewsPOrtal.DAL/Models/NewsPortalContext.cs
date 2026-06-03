using NewsPortal.Common.Models.Requests.Article;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPOrtal.DAL.Models
{
    public class NewsPortalContext : DbContext
    {
        public static string ConnectionStringOverride { get; set; }

        public NewsPortalContext()
            : base(string.IsNullOrWhiteSpace(ConnectionStringOverride)
                ? "name=DefaultConnection"
                : ConnectionStringOverride)
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().ToTable("Articles");
            modelBuilder.Entity<Category>().ToTable("Categories");
        }


       


    }
}
