using MassTransit;
using Microsoft.EntityFrameworkCore;
using Notify.Core.Models;

namespace Notify.Infrastructure.Data;

public class DataDbContext(DbContextOptions<DataDbContext> options) : DbContext(options)
{
    public DbSet<NotifItem> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NotifItem>().ToTable("Notifications");

        modelBuilder.Entity<NotifItem>()
            .Property(n => n.Status)
            .HasConversion<string>();
        modelBuilder.Entity<NotifItem>()
            .Property(n => n.Type)
            .HasConversion<string>();

        //MassTransit outbox
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
