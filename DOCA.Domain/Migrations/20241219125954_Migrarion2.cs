using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOCA.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Migrarion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Members_MemberId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Members",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Members_UserId",
                table: "Members",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Members_MemberId",
                table: "Orders",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Members_MemberId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_UserId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Members");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Members_MemberId",
                table: "Orders",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
