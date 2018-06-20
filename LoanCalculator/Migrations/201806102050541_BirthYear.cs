namespace LoanCalculator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BirthYear : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "BirthYear", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String());
            AddColumn("dbo.AspNetUsers", "Lastname", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Lastname");
            DropColumn("dbo.AspNetUsers", "FirstName");
            DropColumn("dbo.AspNetUsers", "BirthYear");
        }
    }
}
