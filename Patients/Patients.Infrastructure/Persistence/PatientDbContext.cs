using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Patients.Domain.Models;

namespace Patients.Infrastructure.Persistence;

public sealed class PatientDbContext :DbContext
{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Adresse> Adresses { get; set; }
    public DbSet<MedicalInformation> MedicalInformation { get; set; }



    public PatientDbContext()
    {
        ChangeTracker.LazyLoadingEnabled = false;
        ChangeTracker.AutoDetectChangesEnabled = true;
    }

    public PatientDbContext(DbContextOptions<PatientDbContext> options)
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

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patient", schema: "Patient");
            entity.HasKey(a => a.PatientId);
            entity.Property(e => e.PatientId).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedById).HasColumnType("uuid");
            entity.Property(e => e.LastModifiedById).HasColumnType("uuid");
            entity.Property(e => e.Created).HasDefaultValueSql("(NOW())").ValueGeneratedOnAdd();
            entity.Property(e => e.LastModified).HasDefaultValueSql("(NOW())").ValueGeneratedOnAddOrUpdate();

            entity.Property(a => a.FirstName).IsRequired().HasMaxLength(150);
            entity.Property(a => a.LastName).IsRequired().HasMaxLength(150);
            entity.Property(a => a.MiddleName).IsRequired(false).HasMaxLength(150);
            entity.Property(a => a.SocialSecurityNumber).IsRequired(false).HasMaxLength(200);
            entity.Property(a => a.BirthDate).IsRequired();
            entity.Property(a => a.DeathDate).IsRequired(false);
            entity.Property(a => a.Email).HasMaxLength(200);
            entity.Property(a => a.PhoneNumber).HasMaxLength(20);
            entity.Property(a => a.HomeNumber).HasMaxLength(20);
            entity.Property(a => a.Gender).HasConversion<int>();

            entity.HasMany(a => a.Adresses).WithOne(a => a.Patient)
                .HasForeignKey(a => a.FkIdPatient).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(a => a.Contacts).WithOne(a => a.Patient)
                .HasForeignKey(a => a.FkIdPatient).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(a => a.MedicalInformations).WithOne(a => a.Patient)
                .HasForeignKey(a => a.FkIdPatient).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Adresse>(entity =>
        {
            entity.ToTable("Adresse", schema: "Patient");
            entity.HasKey(a => a.AdresseId);
            entity.Property(e => e.AdresseId).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            entity.Property(a => a.Country).HasMaxLength(100);
            entity.Property(a => a.Provence).HasMaxLength(100);
            entity.Property(a => a.City).HasMaxLength(100);
            entity.Property(a => a.PostalCode).HasMaxLength(20);
            entity.Property(a => a.Street).HasMaxLength(255);
            entity.Property(a => a.AdditionalInformation).HasMaxLength(255);
            entity.Property(a => a.Type).HasConversion<int>();

            entity.HasOne(a => a.Patient).WithMany().HasForeignKey(a => a.FkIdPatient)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("Contact", schema: "Patient");
            entity.HasKey(a => a.ContactId);
            entity.Property(e => e.ContactId).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            entity.Property(a => a.FirstName).HasMaxLength(150);
            entity.Property(a => a.LastName).HasMaxLength(150);
            entity.Property(a => a.PhoneNumber).HasMaxLength(20);
            entity.Property(a => a.Email).HasMaxLength(100);
            entity.Property(a => a.Type).HasConversion<int>();

            entity.HasOne(a => a.Patient).WithMany().HasForeignKey(a => a.FkIdPatient)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MedicalInformation>(entity =>
        {
            entity.ToTable("MedicalInformation", schema: "Patient");
            entity.HasKey(a => a.MedicalInformationId);
            entity.Property(e => e.MedicalInformationId).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
            entity.Property(a => a.Name).HasMaxLength(150);
            entity.Property(a => a.Note).HasMaxLength(250);
            entity.Property(a => a.Type).HasConversion<int>();

            entity.HasOne(a => a.Patient).WithMany().HasForeignKey(a => a.FkIdPatient)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

    }

}