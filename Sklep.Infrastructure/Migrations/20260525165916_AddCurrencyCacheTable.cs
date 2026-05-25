using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sklep.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyCacheTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CurrencyCode = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Rate = table.Column<string>(type: "TEXT", nullable: false),
                    FetchedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRates", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 25, 16, 59, 16, 507, DateTimeKind.Utc).AddTicks(1790));

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 25, 16, 59, 16, 507, DateTimeKind.Utc).AddTicks(1800));

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 25, 16, 59, 16, 507, DateTimeKind.Utc).AddTicks(1800));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyExchangeRates");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 20, 11, 19, 5, 675, DateTimeKind.Utc).AddTicks(4680));

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 20, 11, 19, 5, 675, DateTimeKind.Utc).AddTicks(4680));

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 20, 11, 19, 5, 675, DateTimeKind.Utc).AddTicks(4680));
        }
    }
}
