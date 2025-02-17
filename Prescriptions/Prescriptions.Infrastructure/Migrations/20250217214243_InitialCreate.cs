using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prescriptions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    PrescriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ConsultationType = table.Column<int>(type: "integer", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FkPatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkConsultationId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkDoctorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.PrescriptionId);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionEvents",
                columns: table => new
                {
                    PrescriptionEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventDataJson = table.Column<string>(type: "jsonb", nullable: false),
                    FkPrescriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkDoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrescriptionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionEvents", x => x.PrescriptionEventId);
                    table.ForeignKey(
                        name: "FK_PrescriptionEvents_Prescriptions_FkPrescriptionId",
                        column: x => x.FkPrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "PrescriptionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrescriptionEvents_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "PrescriptionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionItems",
                columns: table => new
                {
                    PrescriptionItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    DrugName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Dosage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Frequency = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Duration = table.Column<string>(type: "text", nullable: true),
                    Instructions = table.Column<string>(type: "text", nullable: true),
                    MedicationType = table.Column<int>(type: "integer", nullable: false),
                    IsEssential = table.Column<bool>(type: "boolean", nullable: false),
                    Route = table.Column<int>(type: "integer", nullable: false),
                    TimeOfDay = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MealInstructions = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsPrn = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FkPrescriptionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionItems", x => x.PrescriptionItemId);
                    table.ForeignKey(
                        name: "FK_PrescriptionItems_Prescriptions_FkPrescriptionId",
                        column: x => x.FkPrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "PrescriptionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionEvents_FkPrescriptionId",
                table: "PrescriptionEvents",
                column: "FkPrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionEvents_PrescriptionId",
                table: "PrescriptionEvents",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_FkPrescriptionId",
                table: "PrescriptionItems",
                column: "FkPrescriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrescriptionEvents");

            migrationBuilder.DropTable(
                name: "PrescriptionItems");

            migrationBuilder.DropTable(
                name: "Prescriptions");
        }
    }
}
