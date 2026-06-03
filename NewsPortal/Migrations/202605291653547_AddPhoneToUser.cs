namespace NewsPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddPhoneToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Phone", c => c.String(maxLength: 50));
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Phone");
        }
    }
}
