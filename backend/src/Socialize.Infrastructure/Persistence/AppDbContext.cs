using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Domain.Entities;

namespace Socialize.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostImage> PostImages => Set<PostImage>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Like> Likes => Set<Like>();
    public DbSet<Follow> Follows => Set<Follow>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Generated tsvector columns are Postgres-specific (research R10). Non-Npgsql providers
        // (e.g. EF InMemory in unit tests) can't map NpgsqlTsVector, so drop the shadow property there.
        if (!Database.IsNpgsql())
        {
            modelBuilder.Entity<User>().Ignore("SearchVector");
            modelBuilder.Entity<Post>().Ignore("SearchVector");
        }

        base.OnModelCreating(modelBuilder);
    }
}
