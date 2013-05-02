namespace ChallengeBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstimatedRatings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "WinnerEstimatedRating", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "LoserEstimatedRating", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "LoserEstimatedRating");
            DropColumn("dbo.Matches", "WinnerEstimatedRating");
        }
    }
}
