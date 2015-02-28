namespace ChallengeBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MatchWithdrawn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "Withdrawn", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "Withdrawn");
        }
    }
}
