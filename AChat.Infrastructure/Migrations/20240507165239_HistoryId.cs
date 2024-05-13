using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HistoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HistoryId",
                table: "Source",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ThreadId",
                table: "Message",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8569af18-8997-41aa-a5e2-6ea59cbd557d", "AQAAAAIAAYagAAAAEDM5WK96IA0Z2JIfQw6gk1pLIkIgydDSlngtOOKwFVgJRPOhypNM2bnJ93AQOt00ZA==", "68784492-0a11-4aac-a16b-60234de30190" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HistoryId",
                table: "Source");

            migrationBuilder.DropColumn(
                name: "ThreadId",
                table: "Message");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a4cc82e9-656c-428c-98ae-562f23aee640", "AQAAAAIAAYagAAAAEJK8kd8jUj3etMcKRSnFXi+q5fYfVJcesCljmUC2E5f0IoNOoVbL4zF5xmpCHZulYQ==", "f88520de-44e6-4cdd-8e94-40698e603d58" });
        }
    }
}
