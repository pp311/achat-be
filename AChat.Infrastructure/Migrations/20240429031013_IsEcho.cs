using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IsEcho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEcho",
                table: "Message",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a4cc82e9-656c-428c-98ae-562f23aee640", "AQAAAAIAAYagAAAAEJK8kd8jUj3etMcKRSnFXi+q5fYfVJcesCljmUC2E5f0IoNOoVbL4zF5xmpCHZulYQ==", "f88520de-44e6-4cdd-8e94-40698e603d58" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEcho",
                table: "Message");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5d6048a9-91e1-4203-babd-3e856278be40", "AQAAAAIAAYagAAAAEBtuqMCruvCseze4HhADedJqAshifk1sAGKT2GbdX9PSKPJUNym48ENozZgSGq48/A==", "79ac3ddf-5e92-4094-9a3a-8ad8eb5e9a23" });
        }
    }
}
