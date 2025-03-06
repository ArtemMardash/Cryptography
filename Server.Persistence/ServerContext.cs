using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Server.Domain.Entities;

namespace Server.Persistence;

public class ServerContext: DbContext
{
    
    /// <summary>
    /// Database for users
    /// </summary>
    public DbSet<UserDb> Users { get; set; }
    
    public DbSet<PackageDb> Packages { get; set; }

    public ServerContext(DbContextOptions options): base(options)
    {
        //Database.EnsureCreated();
    }
    
    /// <summary>
    /// Data to db
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserDb>().HasKey(u => u.Id);
        modelBuilder.Entity<UserDb>().Property(u => u.Id).ValueGeneratedNever();
        modelBuilder.Entity<UserDb>().HasMany<PackageDb>(u => u.Packages)
            .WithOne(p => p.Sender)
            .HasForeignKey(p => p.SenderId);

        modelBuilder.Entity<PackageDb>().HasKey(p => p.Id);
        modelBuilder.Entity<PackageDb>().Property(u => u.Id).ValueGeneratedNever();
    }
}

public class UserContextFactory : IDesignTimeDbContextFactory<ServerContext>
{
    public ServerContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ServerContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Server;Username=postgres;Password=postgres",
            builder => builder.MigrationsAssembly(typeof(ServerContext).Assembly.GetName().Name));

        return new ServerContext(optionsBuilder.Options);
    }
}