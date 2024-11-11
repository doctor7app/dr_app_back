using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Patients.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Patient");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "Patient",
                schema: "Patient",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    SocialSecurityNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    LastName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    DeathDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    HomeNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "(NOW())"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "(NOW())"),
                    LastModifiedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.PatientId);
                });

            migrationBuilder.CreateTable(
                name: "Adresse",
                schema: "Patient",
                columns: table => new
                {
                    AdresseId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Provence = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Street = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AdditionalInformation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    FkIdPatient = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adresse", x => x.AdresseId);
                    table.ForeignKey(
                        name: "FK_Adresse_Patient_FkIdPatient",
                        column: x => x.FkIdPatient,
                        principalSchema: "Patient",
                        principalTable: "Patient",
                        principalColumn: "PatientId");
                    table.ForeignKey(
                        name: "FK_Adresse_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "Patient",
                        principalTable: "Patient",
                        principalColumn: "PatientId");
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                schema: "Patient",
                columns: table => new
                {
                    ContactId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    FirstName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    LastName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    FkIdPatient = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_Contact_Patient_FkIdPatient",
                        column: x => x.FkIdPatient,
                        principalSchema: "Patient",
                        principalTable: "Patient",
                        principalColumn: "PatientId");
                    table.ForeignKey(
                        name: "FK_Contact_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "Patient",
                        principalTable: "Patient",
                        principalColumn: "PatientId");
                });

            migrationBuilder.CreateTable(
                name: "MedicalInformation",
                schema: "Patient",
                columns: table => new
                {
                    MedicalInformationId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Note = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    FkIdPatient = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalInformation", x => x.MedicalInformationId);
                    table.ForeignKey(
                        name: "FK_MedicalInformation_Patient_FkIdPatient",
                        column: x => x.FkIdPatient,
                        principalSchema: "Patient",
                        principalTable: "Patient",
                        principalColumn: "PatientId");
                    table.ForeignKey(
                        name: "FK_MedicalInformation_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "Patient",
                        principalTable: "Patient",
                        principalColumn: "PatientId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adresse_FkIdPatient",
                schema: "Patient",
                table: "Adresse",
                column: "FkIdPatient");

            migrationBuilder.CreateIndex(
                name: "IX_Adresse_PatientId",
                schema: "Patient",
                table: "Adresse",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_FkIdPatient",
                schema: "Patient",
                table: "Contact",
                column: "FkIdPatient");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_PatientId",
                schema: "Patient",
                table: "Contact",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalInformation_FkIdPatient",
                schema: "Patient",
                table: "MedicalInformation",
                column: "FkIdPatient");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalInformation_PatientId",
                schema: "Patient",
                table: "MedicalInformation",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adresse",
                schema: "Patient");

            migrationBuilder.DropTable(
                name: "Contact",
                schema: "Patient");

            migrationBuilder.DropTable(
                name: "MedicalInformation",
                schema: "Patient");

            migrationBuilder.DropTable(
                name: "Patient",
                schema: "Patient");
        }
    }
}
