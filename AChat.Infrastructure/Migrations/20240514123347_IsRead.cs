using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IsRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Message",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fe39a1c1-4aa6-437c-840b-979968cd055f", "AQAAAAIAAYagAAAAEBkJUnGYtRe4UJ3CVrIlZfuy2my0gruwx70/8plzj+2p3iFVVu6gP7hFgyI/kqQjaA==", "b2f6fc40-d733-4a5d-8723-82a7c944382f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Message");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b47c791-22df-45de-b25f-6c89370a6522", "AQAAAAIAAYagAAAAEOwQMG7jzu26MdWov68is7U3lg0l/cFiKvp8HTOdfuZSJj3uwiuhWV3vMAFEtIb8MA==", "adab31bc-8deb-485d-a666-bcb1a6b19539" });
        }
    }
}
