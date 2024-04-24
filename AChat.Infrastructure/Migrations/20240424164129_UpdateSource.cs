using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Source",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "SourceId",
                table: "Contact",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefId",
                table: "Contact",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5d6048a9-91e1-4203-babd-3e856278be40", "AQAAAAIAAYagAAAAEBtuqMCruvCseze4HhADedJqAshifk1sAGKT2GbdX9PSKPJUNym48ENozZgSGq48/A==", "79ac3ddf-5e92-4094-9a3a-8ad8eb5e9a23" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Source");

            migrationBuilder.DropColumn(
                name: "RefId",
                table: "Contact");

            migrationBuilder.AlterColumn<int>(
                name: "SourceId",
                table: "Contact",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d8716895-0b20-4939-9bff-e938c809a34f", "AQAAAAIAAYagAAAAENUo02WSEJrX+nTha5EAprho6wjBFxvIZ2j1WvncQuZAjfOiFcg6C1G4GLLDnONlbw==", "693fd8d2-c0fd-4e48-9c20-0c08402481df" });
        }
    }
}
