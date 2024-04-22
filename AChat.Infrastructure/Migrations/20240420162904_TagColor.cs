using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TagColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Tag",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MId",
                table: "Message",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d8716895-0b20-4939-9bff-e938c809a34f", "AQAAAAIAAYagAAAAENUo02WSEJrX+nTha5EAprho6wjBFxvIZ2j1WvncQuZAjfOiFcg6C1G4GLLDnONlbw==", "693fd8d2-c0fd-4e48-9c20-0c08402481df" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "MId",
                table: "Message");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6058c550-2571-45e7-aee6-fcdd1dcc2f5b", "AQAAAAIAAYagAAAAEI1Hp3kk23PyJJ+estKMUNJWf91YnOokYlqHsi3HezMaaZCZeF4Mqmgw9zl/cBJMIw==", "772fdc72-7d05-468f-9210-4cacc1aedcdd" });
        }
    }
}
