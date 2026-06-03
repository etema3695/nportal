namespace NewsPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredmessageforImageTittle : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Articles", "ImageTitle", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Articles", "ImageTitle", c => c.String());
        }
    }
}
