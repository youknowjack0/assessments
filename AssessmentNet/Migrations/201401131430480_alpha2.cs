namespace AssessmentNet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alpha2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestRuns", "HasStarted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestRuns", "HasStarted");
        }
    }
}
