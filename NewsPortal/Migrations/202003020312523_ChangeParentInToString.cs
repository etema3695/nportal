namespace NewsPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeParentInToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Categories", "Parent_Id", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Categories", "Parent_Id", c => c.Int(nullable: false));
        }
    }
}
