using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowRecordsTableV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRecord_AspNetUsers_UserId",
                table: "BorrowRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRecord_Books_BookId",
                table: "BorrowRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BorrowRecord",
                table: "BorrowRecord");

            migrationBuilder.RenameTable(
                name: "BorrowRecord",
                newName: "BorrowRecords");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowRecord_UserId",
                table: "BorrowRecords",
                newName: "IX_BorrowRecords_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowRecord_BookId",
                table: "BorrowRecords",
                newName: "IX_BorrowRecords_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BorrowRecords",
                table: "BorrowRecords",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRecords_AspNetUsers_UserId",
                table: "BorrowRecords",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRecords_Books_BookId",
                table: "BorrowRecords",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRecords_AspNetUsers_UserId",
                table: "BorrowRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRecords_Books_BookId",
                table: "BorrowRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BorrowRecords",
                table: "BorrowRecords");

            migrationBuilder.RenameTable(
                name: "BorrowRecords",
                newName: "BorrowRecord");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowRecords_UserId",
                table: "BorrowRecord",
                newName: "IX_BorrowRecord_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowRecords_BookId",
                table: "BorrowRecord",
                newName: "IX_BorrowRecord_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BorrowRecord",
                table: "BorrowRecord",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRecord_AspNetUsers_UserId",
                table: "BorrowRecord",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRecord_Books_BookId",
                table: "BorrowRecord",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
