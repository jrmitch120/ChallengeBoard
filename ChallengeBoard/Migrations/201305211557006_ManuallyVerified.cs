namespace ChallengeBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManuallyVerified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "ManuallyVerified", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "ManuallyVerified");
        }
    }
}
