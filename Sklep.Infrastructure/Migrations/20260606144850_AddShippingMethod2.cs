using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sklep.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingMethod2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ShippingMethods",
                columns: new[] { "Id", "IsActive", "Name", "Price" },
                values: new object[,]
                {
                    { 1, true, "Kurier DHL", "15" },
                    { 2, true, "Paczkomat", "10" },
                    { 3, true, "Odbiór osobisty", "0" }
                });

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 6, 14, 48, 50, 307, DateTimeKind.Utc).AddTicks(1303));

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 6, 14, 48, 50, 307, DateTimeKind.Utc).AddTicks(1306));

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 6, 14, 48, 50, 307, DateTimeKind.Utc).AddTicks(1307));

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 6, 14, 48, 50, 307, DateTimeKind.Utc).AddTicks(1307));

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 6, 14, 48, 50, 307, DateTimeKind.Utc).AddTicks(1308));

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 6, 14, 48, 50, 307, DateTimeKind.Utc).AddTicks(1308));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ShippingMethods",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ShippingMethods",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ShippingMethods",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 6, 14, 35, 26, 987, DateTimeKind.Utc).AddTicks(1216));

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 6, 14, 35, 26, 987, DateTimeKind.Utc).AddTicks(1221));

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 6, 14, 35, 26, 987, DateTimeKind.Utc).AddTicks(1222));

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 6, 14, 35, 26, 987, DateTimeKind.Utc).AddTicks(1223));

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 6, 14, 35, 26, 987, DateTimeKind.Utc).AddTicks(1223));

            migrationBuilder.UpdateData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 6, 14, 35, 26, 987, DateTimeKind.Utc).AddTicks(1224));
        }
    }
}
