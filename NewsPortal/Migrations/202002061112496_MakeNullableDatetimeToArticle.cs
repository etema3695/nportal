namespace NewsPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeNullableDatetimeToArticle : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Articles", "CreatedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Articles", "CreatedOn", c => c.DateTime(nullable: false));
        }
    }
}
