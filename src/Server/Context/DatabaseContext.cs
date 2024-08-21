using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;

namespace Context;

public class DatabaseContext : DbContext
{
    public DbSet<SystemNode> SystemNodes { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SystemNode>()
            .HasDiscriminator<Models.Type>("NodeType")
            .HasValue<Models.Directory>(Models.Type.Directory)
            .HasValue<Models.File>(Models.Type.File);

        // Additional configurations if needed
    }
}