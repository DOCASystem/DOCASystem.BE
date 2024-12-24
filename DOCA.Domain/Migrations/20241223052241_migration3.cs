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
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AnimalCategories_AnimalCategoryId",
                table: "Animals");

            migrationBuilder.DropIndex(
                name: "IX_Animals_AnimalCategoryId",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "AnimalCategoryId",
                table: "Animals");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Animals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Animals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Animals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AnimalCategories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AnimalCategories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "AnimalCategories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AnimalCategoryRelationship",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalCategoryRelationship", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalCategoryRelationship_AnimalCategories_AnimalCategoryId",
                        column: x => x.AnimalCategoryId,
                        principalTable: "AnimalCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalCategoryRelationship_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalCategoryRelationship_AnimalCategoryId",
                table: "AnimalCategoryRelationship",
                column: "AnimalCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalCategoryRelationship_AnimalId",
                table: "AnimalCategoryRelationship",
                column: "AnimalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalCategoryRelationship");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AnimalCategories");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AnimalCategories");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "AnimalCategories");

            migrationBuilder.AddColumn<Guid>(
                name: "AnimalCategoryId",
                table: "Animals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Animals_AnimalCategoryId",
                table: "Animals",
                column: "AnimalCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AnimalCategories_AnimalCategoryId",
                table: "Animals",
                column: "AnimalCategoryId",
                principalTable: "AnimalCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
