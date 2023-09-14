using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "IsDelete", "Name" },
                values: new object[,]
                {
                    { 1, "Administrator Role", false, "Admin" },
                    { 2, "User Role", false, "User" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Age", "Email", "FullName", "IsActive", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[,]
                {
                    { 1, 30, "alitaami81@gmail.com", "Admin", true, "8jr0nptvznD5VS2WniCx5y6jYyQOSw1ZpfsulA8c/3A=", new Guid("e26b6cdc-fc9e-48f1-997f-1cf3b9feddc7"), "admin" },
                    { 2, 21, "alitaamicr7@gmail.com", "alitaami", true, "8jr0nptvznD5VS2WniCx5y6jYyQOSw1ZpfsulA8c/3A=", new Guid("e6498185-4ad0-4eb6-9604-2b8250da534e"), "ali" }
                });

            migrationBuilder.InsertData(
                table: "NewsReceivers",
                columns: new[] { "Id", "Email", "UserId" },
                values: new object[,]
                {
                    { 1, "alitaami81@gmail.com", 1 },
                    { 2, "alitaamicr7@gmail.com", 2 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NewsReceivers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NewsReceivers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
