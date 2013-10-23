namespace ChallengeBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastViewedPost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Competitors", "LastViewedPostId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Competitors", "LastViewedPostId");
        }
    }
}
