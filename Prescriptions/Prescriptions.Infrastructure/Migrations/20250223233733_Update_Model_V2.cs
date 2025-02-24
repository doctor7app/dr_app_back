using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prescriptions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_Model_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "PrescriptionEvents",
                newName: "OccurredOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccurredOn",
                table: "PrescriptionEvents",
                newName: "Timestamp");
        }
    }
}
