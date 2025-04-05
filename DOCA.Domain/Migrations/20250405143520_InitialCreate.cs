using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOCA.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("651bc8be-6f2c-4793-b286-11fbee866d0c"));

            migrationBuilder.DropColumn(
                name: "WarrantyCode",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "WarrantyExpired",
                table: "OrderItems");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FullName", "Password", "PhoneNumber", "Role", "Username" },
                values: new object[] { new Guid("4580e24a-eb21-410c-a9bc-d8f02db45f4e"), "Admin", "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", "0123456789", "Manager", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4580e24a-eb21-410c-a9bc-d8f02db45f4e"));

            migrationBuilder.AddColumn<string>(
                name: "WarrantyCode",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "WarrantyExpired",
                table: "OrderItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FullName", "Password", "PhoneNumber", "Role", "Username" },
                values: new object[] { new Guid("651bc8be-6f2c-4793-b286-11fbee866d0c"), "Admin", "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", "0123456789", "Manager", "admin" });
        }
    }
}
