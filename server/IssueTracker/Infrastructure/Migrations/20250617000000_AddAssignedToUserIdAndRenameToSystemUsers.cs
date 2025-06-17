using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedToUserIdAndRenameToSystemUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename LoginRequests table to SystemUsers
            migrationBuilder.RenameTable(
                name: "LoginRequests",
                newName: "SystemUsers");

            // Add AssignedToUserId column to Cards table
            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "Cards",
                type: "integer",
                nullable: true);

            // Create foreign key constraint
            migrationBuilder.CreateIndex(
                name: "IX_Cards_AssignedToUserId",
                table: "Cards",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_SystemUsers_AssignedToUserId",
                table: "Cards",
                column: "AssignedToUserId",
                principalTable: "SystemUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove foreign key constraint and index
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_SystemUsers_AssignedToUserId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_AssignedToUserId",
                table: "Cards");

            // Remove AssignedToUserId column from Cards table
            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Cards");

            // Rename SystemUsers table back to LoginRequests
            migrationBuilder.RenameTable(
                name: "SystemUsers",
                newName: "LoginRequests");
        }
    }
}
