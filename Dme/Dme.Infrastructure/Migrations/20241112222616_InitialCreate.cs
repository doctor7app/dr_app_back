using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dme.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Dme");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "Dme",
                schema: "Dme",
                columns: table => new
                {
                    DmeId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AdditionalInformations = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    FkIdPatient = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    FkIdDoctor = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "(NOW())"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(NOW())"),
                    LastModifiedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dme", x => x.DmeId);
                });

            migrationBuilder.CreateTable(
                name: "Consultations",
                schema: "Dme",
                columns: table => new
                {
                    ConsultationId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    ReasonOfVisit = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Symptoms = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Weight = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    Height = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    PressureArterial = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    Temperature = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    CardiacFrequency = table.Column<int>(type: "integer", nullable: true),
                    SaturationOxygen = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ConsultationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "(NOW())"),
                    NextConsultationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    FkIdDme = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "(NOW())"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "(NOW())"),
                    LastModifiedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultations", x => x.ConsultationId);
                    table.ForeignKey(
                        name: "FK_Consultations_Dme_FkIdDme",
                        column: x => x.FkIdDme,
                        principalSchema: "Dme",
                        principalTable: "Dme",
                        principalColumn: "DmeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Diagnostics",
                schema: "Dme",
                columns: table => new
                {
                    DiagnosticId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    TypeDiagnostic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Results = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Comments = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FkIdConsultation = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnostics", x => x.DiagnosticId);
                    table.ForeignKey(
                        name: "FK_Diagnostics_Consultations_FkIdConsultation",
                        column: x => x.FkIdConsultation,
                        principalSchema: "Dme",
                        principalTable: "Consultations",
                        principalColumn: "ConsultationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Treatments",
                schema: "Dme",
                columns: table => new
                {
                    TreatmentsId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Medicament = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Dose = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Frequency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Duration = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Instructions = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FkIdConsultation = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treatments", x => x.TreatmentsId);
                    table.ForeignKey(
                        name: "FK_Treatments_Consultations_FkIdConsultation",
                        column: x => x.FkIdConsultation,
                        principalSchema: "Dme",
                        principalTable: "Consultations",
                        principalColumn: "ConsultationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_FkIdDme",
                schema: "Dme",
                table: "Consultations",
                column: "FkIdDme");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostics_FkIdConsultation",
                schema: "Dme",
                table: "Diagnostics",
                column: "FkIdConsultation");

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_FkIdConsultation",
                schema: "Dme",
                table: "Treatments",
                column: "FkIdConsultation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnostics",
                schema: "Dme");

            migrationBuilder.DropTable(
                name: "Treatments",
                schema: "Dme");

            migrationBuilder.DropTable(
                name: "Consultations",
                schema: "Dme");

            migrationBuilder.DropTable(
                name: "Dme",
                schema: "Dme");
        }
    }
}
