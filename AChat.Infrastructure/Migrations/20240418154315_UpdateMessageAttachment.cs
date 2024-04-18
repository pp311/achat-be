using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessageAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cd8bdc29-6164-4e12-9c16-1ab1c6d319db", "AQAAAAIAAYagAAAAEBYW+DaATb0gmgKF/5AOyrYSbdAAdVFwkoknYT9a4Vq8E9TRuCKKrRrjzC88v8zlcg==", "8ccdbd07-9cf2-464f-89a9-4454f5d0dcb0" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f66361a5-6bb4-4429-b0a1-fa977a60e582", "AQAAAAIAAYagAAAAEEkrao5cLlRzlnpitiCSe5oGjXh3J6M0eB8Ntza5aTdJMt3eMDShLBPIQ5jsPz7ELg==", "4844bbb5-8f78-4425-9273-ef0c5a390b54" });
        }
    }
}
