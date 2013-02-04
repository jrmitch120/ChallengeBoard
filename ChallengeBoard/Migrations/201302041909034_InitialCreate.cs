namespace ChallengeBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        MatchId = c.Int(nullable: false, identity: true),
                        WinnerRatingDelta = c.Int(nullable: false),
                        LoserRatingDelta = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        VerificationDeadline = c.DateTime(nullable: false),
                        Resolved = c.DateTime(),
                        Tied = c.Boolean(nullable: false),
                        Verified = c.Boolean(nullable: false),
                        Rejected = c.Boolean(nullable: false),
                        Board_BoardId = c.Int(),
                        Board_BoardId1 = c.Int(),
                        Winner_CompetitorId = c.Int(),
                        Loser_CompetitorId = c.Int(),
                    })
                .PrimaryKey(t => t.MatchId)
                .ForeignKey("dbo.Boards", t => t.Board_BoardId, cascadeDelete: true)
                .ForeignKey("dbo.Boards", t => t.Board_BoardId1)
                .ForeignKey("dbo.Competitors", t => t.Winner_CompetitorId)
                .ForeignKey("dbo.Competitors", t => t.Loser_CompetitorId)
                .Index(t => t.Board_BoardId)
                .Index(t => t.Board_BoardId1)
                .Index(t => t.Winner_CompetitorId)
                .Index(t => t.Loser_CompetitorId);
            
            CreateTable(
                "dbo.Boards",
                c => new
                    {
                        BoardId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 45),
                        Description = c.String(),
                        Password = c.String(maxLength: 32),
                        Created = c.DateTime(nullable: false),
                        StartingRating = c.Int(nullable: false),
                        AutoVerification = c.Int(nullable: false),
                        End = c.DateTime(nullable: false),
                        Started = c.DateTime(nullable: false),
                        Owner_CompetitorId = c.Int(),
                    })
                .PrimaryKey(t => t.BoardId)
                .ForeignKey("dbo.Competitors", t => t.Owner_CompetitorId)
                .Index(t => t.Owner_CompetitorId);
            
            CreateTable(
                "dbo.Competitors",
                c => new
                    {
                        CompetitorId = c.Int(nullable: false, identity: true),
                        ProfileUserId = c.Int(nullable: false),
                        Name = c.String(),
                        Status = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        Wins = c.Int(nullable: false),
                        Loses = c.Int(nullable: false),
                        Ties = c.Int(nullable: false),
                        Streak = c.Int(nullable: false),
                        RejectionsReceived = c.Int(nullable: false),
                        RejectionsGiven = c.Int(nullable: false),
                        Board_BoardId = c.Int(),
                    })
                .PrimaryKey(t => t.CompetitorId)
                .ForeignKey("dbo.UserProfile", t => t.ProfileUserId, cascadeDelete: true)
                .ForeignKey("dbo.Boards", t => t.Board_BoardId, cascadeDelete: true)
                .Index(t => t.ProfileUserId)
                .Index(t => t.Board_BoardId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        EmailAddress = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Competitors", new[] { "Board_BoardId" });
            DropIndex("dbo.Competitors", new[] { "ProfileUserId" });
            DropIndex("dbo.Boards", new[] { "Owner_CompetitorId" });
            DropIndex("dbo.Matches", new[] { "Loser_CompetitorId" });
            DropIndex("dbo.Matches", new[] { "Winner_CompetitorId" });
            DropIndex("dbo.Matches", new[] { "Board_BoardId1" });
            DropIndex("dbo.Matches", new[] { "Board_BoardId" });
            DropForeignKey("dbo.Competitors", "Board_BoardId", "dbo.Boards");
            DropForeignKey("dbo.Competitors", "ProfileUserId", "dbo.UserProfile");
            DropForeignKey("dbo.Boards", "Owner_CompetitorId", "dbo.Competitors");
            DropForeignKey("dbo.Matches", "Loser_CompetitorId", "dbo.Competitors");
            DropForeignKey("dbo.Matches", "Winner_CompetitorId", "dbo.Competitors");
            DropForeignKey("dbo.Matches", "Board_BoardId1", "dbo.Boards");
            DropForeignKey("dbo.Matches", "Board_BoardId", "dbo.Boards");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Competitors");
            DropTable("dbo.Boards");
            DropTable("dbo.Matches");
        }
    }
}
