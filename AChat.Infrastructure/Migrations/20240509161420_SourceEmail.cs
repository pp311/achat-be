using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SourceEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Source",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b47c791-22df-45de-b25f-6c89370a6522", "AQAAAAIAAYagAAAAEOwQMG7jzu26MdWov68is7U3lg0l/cFiKvp8HTOdfuZSJj3uwiuhWV3vMAFEtIb8MA==", "adab31bc-8deb-485d-a666-bcb1a6b19539" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Source");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8569af18-8997-41aa-a5e2-6ea59cbd557d", "AQAAAAIAAYagAAAAEDM5WK96IA0Z2JIfQw6gk1pLIkIgydDSlngtOOKwFVgJRPOhypNM2bnJ93AQOt00ZA==", "68784492-0a11-4aac-a16b-60234de30190" });
        }
    }
}
