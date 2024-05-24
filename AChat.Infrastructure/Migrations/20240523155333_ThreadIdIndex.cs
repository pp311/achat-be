using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThreadIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ThreadId",
                table: "Message",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MId",
                table: "Message",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d5583e7d-3e9c-4ec1-b505-76643f0dc8cb", "AQAAAAIAAYagAAAAEIiNMAUdVc1aF6g0ClPhsfOOHgmKWEzo1m3qdHt18jwYdJZ6NxGqhjfa7MErihG40w==", "48d2f8a0-cd25-44ae-be8a-f3ced52558ba" });

            migrationBuilder.CreateIndex(
                name: "IX_Message_ThreadId",
                table: "Message",
                column: "ThreadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Message_ThreadId",
                table: "Message");

            migrationBuilder.AlterColumn<string>(
                name: "ThreadId",
                table: "Message",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MId",
                table: "Message",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fe39a1c1-4aa6-437c-840b-979968cd055f", "AQAAAAIAAYagAAAAEBkJUnGYtRe4UJ3CVrIlZfuy2my0gruwx70/8plzj+2p3iFVVu6gP7hFgyI/kqQjaA==", "b2f6fc40-d733-4a5d-8723-82a7c944382f" });
        }
    }
}
