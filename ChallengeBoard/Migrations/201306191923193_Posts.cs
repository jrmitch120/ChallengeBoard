namespace ChallengeBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Posts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        PostId = c.Int(nullable: false, identity: true),
                        Body = c.String(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Edited = c.DateTime(),
                        Board_BoardId = c.Int(),
                        Owner_CompetitorId = c.Int(),
                    })
                .PrimaryKey(t => t.PostId)
                .ForeignKey("dbo.Boards", t => t.Board_BoardId)
                .ForeignKey("dbo.Competitors", t => t.Owner_CompetitorId)
                .Index(t => t.Board_BoardId)
                .Index(t => t.Owner_CompetitorId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Posts", new[] { "Owner_CompetitorId" });
            DropIndex("dbo.Posts", new[] { "Board_BoardId" });
            DropForeignKey("dbo.Posts", "Owner_CompetitorId", "dbo.Competitors");
            DropForeignKey("dbo.Posts", "Board_BoardId", "dbo.Boards");
            DropTable("dbo.Posts");
        }
    }
}
