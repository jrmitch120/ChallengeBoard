namespace ChallengeBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompetitorJoin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Competitors", "Joined", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Competitors", "Joined");
        }
    }
}
