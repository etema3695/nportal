namespace NewsPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageToArticle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "ImageTitle", c => c.String());
            AddColumn("dbo.Articles", "ImagePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Articles", "ImagePath");
            DropColumn("dbo.Articles", "ImageTitle");
        }
    }
}
