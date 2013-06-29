using System.Data.Entity;

namespace ChallengeBoard.Models
{
    public class ChallengeBoardContext : DbContext
    {
        public ChallengeBoardContext() 
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Competitor>().HasRequired(p => p.Board);
        // http://msdn.microsoft.com/en-us/data/hh134698.aspx

            //modelBuilder.Entity<Board>().HasMany(b => b.Competitors);

            //http://weblogs.asp.net/manavi/archive/2011/03/27/associations-in-ef-4-1-code-first-part-1-introduction-and-basic-concepts.aspx

            modelBuilder.Entity<Competitor>()
                        .HasRequired(a => a.Profile)
                        .WithMany()
                        .HasForeignKey(u => u.ProfileUserId);

            //http://stackoverflow.com/questions/5471374/how-do-you-ensure-cascade-delete-is-enabled-on-a-table-relationship-in-ef-code-f
            modelBuilder.Entity<Board>()
                        .HasMany(b => b.Matches)
                        .WithOptional();
                        //.WillCascadeOnDelete(true); Once matches roll in, boards can't be deleted.

            modelBuilder.Entity<Board>()
                        .HasMany(b => b.Competitors)
                        .WithOptional()
                        .WillCascadeOnDelete(true);

            modelBuilder.Entity<Match>().HasRequired<Board>(m => m.Board);

            //modelBuilder.Entity<Competitor>()
            //   .HasOptional(r => r.Profile)
            //   .WithMany()
            //   .HasForeignKey(r => r.ProfileUserId);

            //modelBuilder.Entity<Match>()
            //            .HasRequired(a => a.Loser)
            //            .WithMany()
            //            .HasForeignKey(u => u.LoserId);

            //base.OnModelCreating(modelBuilder);
        }

        public DbSet<Match> Matches { get; set; }
        public DbSet<Competitor> Competitors { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Post> Posts { get; set; }

        // MVC Authentication
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}