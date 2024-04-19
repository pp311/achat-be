using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSourceModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PageName",
                table: "Source",
                newName: "Name");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6058c550-2571-45e7-aee6-fcdd1dcc2f5b", "AQAAAAIAAYagAAAAEI1Hp3kk23PyJJ+estKMUNJWf91YnOokYlqHsi3HezMaaZCZeF4Mqmgw9zl/cBJMIw==", "772fdc72-7d05-468f-9210-4cacc1aedcdd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Source",
                newName: "PageName");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cd8bdc29-6164-4e12-9c16-1ab1c6d319db", "AQAAAAIAAYagAAAAEBYW+DaATb0gmgKF/5AOyrYSbdAAdVFwkoknYT9a4Vq8E9TRuCKKrRrjzC88v8zlcg==", "8ccdbd07-9cf2-464f-89a9-4454f5d0dcb0" });
        }
    }
}
