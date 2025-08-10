using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prescriptions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_Model_V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrescriptionEvents");

            migrationBuilder.CreateTable(
                name: "StoredEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "jsonb", nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AggregateId = table.Column<Guid>(type: "uuid", nullable: false),
                    AggregateType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredEvents", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoredEvents");

            migrationBuilder.CreateTable(
                name: "PrescriptionEvents",
                columns: table => new
                {
                    PrescriptionEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrescriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventDataJson = table.Column<string>(type: "jsonb", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    FkDoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkPrescriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionEvents_FkPrescriptionId",
                table: "PrescriptionEvents",
                column: "FkPrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionEvents_PrescriptionId",
                table: "PrescriptionEvents",
                column: "PrescriptionId");
        }
    }
}
