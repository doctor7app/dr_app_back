using Dme.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Dme.Infrastructure.Persistence;

public sealed class DmeDbContext : DbContext
{
    public DbSet<Consultations> Consultations { get; set; }
    public DbSet<Domain.Models.Dme> Dmes { get; set; }
    public DbSet<Diagnostics> Diagnostics { get; set; }
    public DbSet<Treatments> Treatments { get; set; }

    public DmeDbContext()
    {
        ChangeTracker.LazyLoadingEnabled = false;
        ChangeTracker.AutoDetectChangesEnabled = true;
    }

    public DmeDbContext(DbContextOptions<DmeDbContext> options)
        : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
        ChangeTracker.AutoDetectChangesEnabled = true;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //    if (optionsBuilder.IsConfigured) return;
        //    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        //    string strConnection = builder.Build().GetSection("ConnectionStrings")["MyDbPost"];
        //    optionsBuilder.UseNpgsql(strConnection);
        //    optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Domain.Models.Dme>(entity =>
        {
            entity.ToTable("Dme", schema: "Dme");
            entity.HasKey(a => a.DmeId);
            entity.Property(e => e.DmeId).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            entity.Property(e => e.FkIdDoctor).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()").IsRequired();
            entity.Property(e => e.FkIdPatient).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()").IsRequired();
            entity.Property(e => e.CreatedById).HasColumnType("uuid");
            entity.Property(e => e.LastModifiedById).HasColumnType("uuid");
            entity.Property(a => a.Notes).IsRequired(false).HasMaxLength(255);
            entity.Property(a => a.AdditionalInformations).IsRequired(false).HasMaxLength(150);
            entity.Property(e => e.Created).HasDefaultValueSql("(NOW())").ValueGeneratedOnAdd();
            entity.Property(e => e.LastModified).HasDefaultValueSql("(NOW())").ValueGeneratedOnAddOrUpdate();
            entity.Property(a => a.State).HasConversion<int>();

            entity.HasMany(a => a.Consultations).WithOne(a => a.Dme)
                .HasForeignKey(a => a.FkIdDme).OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Consultations>(entity =>
        {
            entity.ToTable("Consultations", schema: "Dme");
            entity.HasKey(a => a.ConsultationId);
            entity.Property(e => e.ConsultationId).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedById).HasColumnType("uuid");
            entity.Property(e => e.LastModifiedById).HasColumnType("uuid");
            entity.Property(e => e.Created).HasColumnType("datetime").HasDefaultValueSql("(NOW())").ValueGeneratedOnAdd();
            entity.Property(e => e.LastModified).HasColumnType("datetime").HasDefaultValueSql("(NOW())").ValueGeneratedOnAddOrUpdate();

            entity.Property(a => a.ReasonOfVisit).IsRequired(false).HasMaxLength(255);
            entity.Property(a => a.Symptoms).IsRequired(false).HasMaxLength(150);
            entity.Property(a => a.Weight).IsRequired(false).HasColumnType("decimal(5, 2)");
            entity.Property(a => a.Height).IsRequired(false).HasColumnType("decimal(5, 2)");
            entity.Property(a => a.PressureArterial).IsRequired(false).HasColumnType("decimal(5, 2)");
            entity.Property(a => a.Temperature).IsRequired(false).HasColumnType("decimal(5, 2)");
            entity.Property(a => a.CardiacFrequency).IsRequired(false);
            entity.Property(a => a.SaturationOxygen).IsRequired(false).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ConsultationDate).HasColumnType("datetime").HasDefaultValueSql("(NOW())").IsRequired();
            entity.Property(e => e.NextConsultationDate).HasColumnType("datetime").IsRequired(false);

            entity.Property(a => a.Type).HasConversion<int>();
            entity.Property(a => a.State).HasConversion<int>();

            entity.HasOne(d => d.Dme)
                .WithMany(p => p.Consultations)
                .HasForeignKey(d => d.FkIdDme)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.Diagnostics).WithOne(a => a.Consultation)
                .HasForeignKey(a => a.FkIdConsultation).OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.Treatments).WithOne(a => a.Consultation)
                .HasForeignKey(a => a.FkIdConsultation).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Diagnostics>(entity =>
        {
            entity.ToTable("Diagnostics", schema: "Dme");
            entity.HasKey(a => a.DiagnosticId);
            entity.Property(e => e.DiagnosticId).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            entity.Property(e => e.TypeDiagnostic).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Results).HasMaxLength(500);
            entity.Property(e => e.Comments).HasMaxLength(500);

            entity.HasOne(d => d.Consultation)
                .WithMany(c => c.Diagnostics)
                .HasForeignKey(d => d.FkIdConsultation)
                .OnDelete(DeleteBehavior.Cascade);
            
        });

        modelBuilder.Entity<Treatments>(entity =>
        {
            entity.ToTable("Treatments", schema: "Dme");
            entity.HasKey(a => a.TreatmentsId);
            entity.Property(e => e.TreatmentsId).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            entity.Property(e => e.Medicament).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Dose).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Frequency).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Duration).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Instructions).HasMaxLength(500);

            entity.HasOne(t => t.Consultation)
                .WithMany(c => c.Treatments)
                .HasForeignKey(t => t.FkIdConsultation)
                .OnDelete(DeleteBehavior.Cascade);
            
        });
    }
}