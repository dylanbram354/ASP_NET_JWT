using Microsoft.EntityFrameworkCore.Migrations;

namespace JWT_API.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f558ee39-18ab-43e1-b4ca-71641a9a4cd6", "8c18e101-e198-4c3a-805c-a2e8501f2e3f", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7f3a6fc3-5296-48cd-9c30-edabb43c2ec6", "9129edb7-0085-431a-89aa-fc2080d1cd3c", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7f3a6fc3-5296-48cd-9c30-edabb43c2ec6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f558ee39-18ab-43e1-b4ca-71641a9a4cd6");
        }
    }
}
