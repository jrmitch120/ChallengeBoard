namespace ChallengeBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogoUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boards", "LogoUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Boards", "LogoUrl");
        }
    }
}
