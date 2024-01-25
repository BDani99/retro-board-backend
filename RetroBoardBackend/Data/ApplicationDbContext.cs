using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RetroBoardBackend.Models;

namespace RetroBoardBackend.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int,
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public override DbSet<User> Users { get; set; }
        public override DbSet<UserRole> UserRoles { get; set; }
        public override DbSet<Role> Roles { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Retrospective> Retrospectives { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<EntryReaction> EntryReactions { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId);

            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            modelBuilder.Ignore<IdentityUserClaim<int>>();
            modelBuilder.Ignore<IdentityUserLogin<int>>();
            modelBuilder.Ignore<IdentityUserToken<int>>();
            modelBuilder.Ignore<IdentityRoleClaim<int>>();

            modelBuilder.Entity<Project>()
               .HasMany(x => x.Users)
               .WithMany(e => e.Projects)
               .UsingEntity(
                x => x.HasOne(typeof(User)).WithMany().HasForeignKey("UsersId").HasPrincipalKey(nameof(User.Id)),
                y => y.HasOne(typeof(Project)).WithMany().HasForeignKey("ProjectsId").HasPrincipalKey(nameof(Project.Id)),
                j => j.HasKey("ProjectsId", "UsersId"));

            //modelBuilder.Entity<Project>()
            //     .HasMany(x => x.Users)
            //     .WithMany(x => x.Projects);

            // modelBuilder.Entity<User>()
            //     .HasMany(x => x.Projects)
            //     .WithMany(x => x.Users);

            modelBuilder.Entity<Project>()
                .HasOne(x => x.AuthorUser)
                .WithMany(x => x.ProjectsWhereAuthor)
                .HasForeignKey(x => x.PmId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Retrospective>()
                .HasOne(x => x.Project)
                .WithMany(x => x.Retrospectives)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Entry>()
                .HasOne(x => x.Author)
                .WithMany(x => x.EntriesWhereAuthor)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Entry>()
                .HasOne(x => x.Assignee)
                .WithMany(x => x.EntriesWhereAssignee)
                .HasForeignKey(x => x.AssigneeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Entry>()
                .HasOne(x => x.Retrospective)
                .WithMany(x => x.Entries)
                .HasForeignKey(x => x.RetrospectiveId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EntryReaction>()
                .HasOne(x => x.User)
                .WithMany(x => x.EntryReactions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EntryReaction>()
                .HasOne(x => x.Entry)
                .WithMany(x => x.EntryReactions)
                .HasForeignKey(x => x.EntryId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
