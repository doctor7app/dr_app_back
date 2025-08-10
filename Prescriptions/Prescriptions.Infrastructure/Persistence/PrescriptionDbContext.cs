using Microsoft.EntityFrameworkCore;
using Prescriptions.Domain.Models;
using Microsoft.Extensions.Configuration;
using MassTransit;

namespace Prescriptions.Infrastructure.Persistence;

public sealed class PrescriptionDbContext : DbContext
{
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
    //public DbSet<PrescriptionEvent> PrescriptionEvents { get; set; }
    public DbSet<StoredEvent> StoredEvents { get; set; }

    public PrescriptionDbContext()
    {
        ChangeTracker.LazyLoadingEnabled = false;
        ChangeTracker.AutoDetectChangesEnabled = true;
    }

    public PrescriptionDbContext(DbContextOptions<PrescriptionDbContext> options)
        : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
        ChangeTracker.AutoDetectChangesEnabled = true;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        string strConnection = builder.Build().GetSection("ConnectionStrings")["MyDbPost"];
        optionsBuilder.UseNpgsql(strConnection);
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        modelBuilder.HasPostgresExtension("uuid-ossp");
        // Configuration de Prescription
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(p => p.PrescriptionId);

            entity.Property(p => p.IssuedAt)
                .IsRequired();

            entity.Property(p => p.Status)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(p => p.ConsultationType)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(p => p.Notes)
                .HasMaxLength(1000);

            entity.Property(p => p.FkPatientId)
                .IsRequired();

            entity.Property(p => p.FkDoctorId)
                .IsRequired();

            entity.Property(p => p.FkConsultationId)
                .IsRequired();

            // Relations
            entity.HasMany(p => p.Items)
                .WithOne(i => i.Prescription)
                .HasForeignKey(i => i.FkPrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration de PrescriptionItem
        modelBuilder.Entity<PrescriptionItem>(entity =>
        {
            entity.HasKey(i => i.PrescriptionItemId);

            entity.Property(i => i.DrugName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(i => i.Dosage)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(i => i.Frequency)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(i => i.MedicationType)
                .HasConversion<int>();

            entity.Property(i => i.Route)
                .HasConversion<int>();

            entity.Property(i => i.TimeOfDay)
                .HasMaxLength(50);

            entity.Property(i => i.MealInstructions)
                .HasMaxLength(100);

            entity.Property(i => i.Notes)
                .HasMaxLength(500);
        });

        // Configuration de StoredEvent
        modelBuilder.Entity<StoredEvent>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OccurredOn)
                .IsRequired();

            entity.Property(e => e.EventType)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.Data)
                .IsRequired()
                .HasColumnType("jsonb");

        });
        
    }
}