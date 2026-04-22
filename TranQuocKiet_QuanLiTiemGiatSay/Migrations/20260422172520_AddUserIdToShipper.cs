using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranQuocKiet_QuanLiTiemGiatSay.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToShipper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Shippers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shippers_UserId",
                table: "Shippers",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Shippers_Users_UserId",
                table: "Shippers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shippers_Users_UserId",
                table: "Shippers");

            migrationBuilder.DropIndex(
                name: "IX_Shippers_UserId",
                table: "Shippers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Shippers");
        }
    }
}
