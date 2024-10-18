using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagement.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDocumentTagsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTag_Documents_DocumentId",
                table: "DocumentTag");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTag_Tags_TagId",
                table: "DocumentTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentTag",
                table: "DocumentTag");

            migrationBuilder.RenameTable(
                name: "DocumentTag",
                newName: "DocumentTags");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentTag_TagId",
                table: "DocumentTags",
                newName: "IX_DocumentTags_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentTags",
                table: "DocumentTags",
                columns: new[] { "DocumentId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTags_Documents_DocumentId",
                table: "DocumentTags",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTags_Tags_TagId",
                table: "DocumentTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTags_Documents_DocumentId",
                table: "DocumentTags");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTags_Tags_TagId",
                table: "DocumentTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentTags",
                table: "DocumentTags");

            migrationBuilder.RenameTable(
                name: "DocumentTags",
                newName: "DocumentTag");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentTags_TagId",
                table: "DocumentTag",
                newName: "IX_DocumentTag_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentTag",
                table: "DocumentTag",
                columns: new[] { "DocumentId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTag_Documents_DocumentId",
                table: "DocumentTag",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTag_Tags_TagId",
                table: "DocumentTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
