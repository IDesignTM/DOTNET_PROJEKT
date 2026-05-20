using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sklep.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddManyToManyTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "CreatedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 20, 11, 19, 5, 675, DateTimeKind.Utc).AddTicks(4680), "Nowość" },
                    { 2, new DateTime(2026, 5, 20, 11, 19, 5, 675, DateTimeKind.Utc).AddTicks(4680), "Bestseller" },
                    { 3, new DateTime(2026, 5, 20, 11, 19, 5, 675, DateTimeKind.Utc).AddTicks(4680), "Wyprzedaż" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
