﻿// <auto-generated />
using System;
using DocumentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DocumentManagement.Migrations
{
    [DbContext(typeof(DocumentDbContext))]
    partial class DocumentDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DocumentManagement.Core.Models.Document", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ContentType")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<string>("FilePath")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Documents", (string)null);
                });

            modelBuilder.Entity("DocumentManagement.Core.Models.DocumentTag", b =>
                {
                    b.Property<Guid>("DocumentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TagId")
                        .HasColumnType("uuid");

                    b.HasKey("DocumentId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("DocumentTag");
                });

            modelBuilder.Entity("DocumentManagement.Core.Models.Metadata", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Author")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid>("DocumentId")
                        .HasColumnType("uuid");

                    b.Property<bool>("Encrypted")
                        .HasColumnType("boolean");

                    b.Property<long>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("Service")
                        .HasColumnType("text");

                    b.Property<DateTime>("UploadedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId")
                        .IsUnique();

                    b.ToTable("Metadatas", (string)null);
                });

            modelBuilder.Entity("DocumentManagement.Core.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tags", (string)null);
                });

            modelBuilder.Entity("DocumentManagement.Core.Models.DocumentTag", b =>
                {
                    b.HasOne("DocumentManagement.Core.Models.Document", "Document")
                        .WithMany("DocumentTags")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DocumentManagement.Core.Models.Tag", "Tag")
                        .WithMany("DocumentTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Document");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("DocumentManagement.Core.Models.Metadata", b =>
                {
                    b.HasOne("DocumentManagement.Core.Models.Document", "Document")
                        .WithOne("Metadata")
                        .HasForeignKey("DocumentManagement.Core.Models.Metadata", "DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Document");
                });

            modelBuilder.Entity("DocumentManagement.Core.Models.Document", b =>
                {
                    b.Navigation("DocumentTags");

                    b.Navigation("Metadata");
                });

            modelBuilder.Entity("DocumentManagement.Core.Models.Tag", b =>
                {
                    b.Navigation("DocumentTags");
                });
#pragma warning restore 612, 618
        }
    }
}
