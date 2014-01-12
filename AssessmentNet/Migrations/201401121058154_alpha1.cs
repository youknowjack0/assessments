namespace AssessmentNet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alpha1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsCorrect = c.Boolean(),
                        AnswerHtml = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Question_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .Index(t => t.Question_Id);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionHtml = c.String(),
                        Weight = c.Int(nullable: false),
                        AllowedTime = c.Time(nullable: false, precision: 7),
                        Test_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tests", t => t.Test_Id)
                .Index(t => t.Test_Id);
            
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Creator = c.String(nullable: false),
                        Created = c.DateTime(nullable: false),
                        MaxDurationInHours = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuestionResponses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HasStarted = c.Boolean(nullable: false),
                        Started = c.DateTime(nullable: false),
                        HasFinished = c.Boolean(nullable: false),
                        Finished = c.DateTime(nullable: false),
                        Question_Id = c.Int(),
                        TestRun_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .ForeignKey("dbo.TestRuns", t => t.TestRun_Id)
                .Index(t => t.Question_Id)
                .Index(t => t.TestRun_Id);
            
            CreateTable(
                "dbo.QuestionResponseAnswers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Answer_Id = c.Int(nullable: false),
                        Response_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Answers", t => t.Answer_Id, cascadeDelete: true)
                .ForeignKey("dbo.QuestionResponses", t => t.Response_Id, cascadeDelete: true)
                .Index(t => t.Answer_Id)
                .Index(t => t.Response_Id);
            
            CreateTable(
                "dbo.TestRuns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Created = c.DateTime(nullable: false),
                        Started = c.DateTime(nullable: false),
                        Expires = c.DateTime(nullable: false),
                        Test_Id = c.Int(),
                        Testee_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tests", t => t.Test_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Testee_Id)
                .Index(t => t.Test_Id)
                .Index(t => t.Testee_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestRuns", "Testee_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TestRuns", "Test_Id", "dbo.Tests");
            DropForeignKey("dbo.QuestionResponses", "TestRun_Id", "dbo.TestRuns");
            DropForeignKey("dbo.QuestionResponses", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.QuestionResponseAnswers", "Response_Id", "dbo.QuestionResponses");
            DropForeignKey("dbo.QuestionResponseAnswers", "Answer_Id", "dbo.Answers");
            DropForeignKey("dbo.Questions", "Test_Id", "dbo.Tests");
            DropForeignKey("dbo.Answers", "Question_Id", "dbo.Questions");
            DropIndex("dbo.TestRuns", new[] { "Testee_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.TestRuns", new[] { "Test_Id" });
            DropIndex("dbo.QuestionResponses", new[] { "TestRun_Id" });
            DropIndex("dbo.QuestionResponses", new[] { "Question_Id" });
            DropIndex("dbo.QuestionResponseAnswers", new[] { "Response_Id" });
            DropIndex("dbo.QuestionResponseAnswers", new[] { "Answer_Id" });
            DropIndex("dbo.Questions", new[] { "Test_Id" });
            DropIndex("dbo.Answers", new[] { "Question_Id" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.TestRuns");
            DropTable("dbo.QuestionResponseAnswers");
            DropTable("dbo.QuestionResponses");
            DropTable("dbo.Tests");
            DropTable("dbo.Questions");
            DropTable("dbo.Answers");
        }
    }
}
