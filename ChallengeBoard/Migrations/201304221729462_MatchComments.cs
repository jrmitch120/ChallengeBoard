namespace ChallengeBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MatchComments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "WinnerComment", c => c.String(maxLength: 140));
            AddColumn("dbo.Matches", "LoserComment", c => c.String(maxLength: 140));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "LoserComment");
            DropColumn("dbo.Matches", "WinnerComment");
        }
    }
}
