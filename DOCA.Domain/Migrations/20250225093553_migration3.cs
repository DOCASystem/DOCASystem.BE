using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOCA.Domain.Migrations
{
    /// <inheritdoc />
    public partial class migration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d57e9981-c3d1-4191-b32a-1aa2f5526c00"));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FullName", "Password", "PhoneNumber", "Role", "Username" },
                values: new object[] { new Guid("82811516-70d1-49e7-b887-43c0bc9d72d3"), "Admin", "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", "0123456789", "Manager", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("82811516-70d1-49e7-b887-43c0bc9d72d3"));

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Blogs");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FullName", "Password", "PhoneNumber", "Role", "Username" },
                values: new object[] { new Guid("d57e9981-c3d1-4191-b32a-1aa2f5526c00"), "Admin", "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", "0123456789", "Manager", "admin" });
        }
    }
}
