using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Customers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3480), null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3488), null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3489), null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3311), new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3313) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3320), new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3320) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3321), new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3322) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3322), new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3323) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3323), new DateTime(2026, 5, 16, 10, 34, 54, 264, DateTimeKind.Utc).AddTicks(3324) });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Email",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Customers");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(7133));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(7136));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(7137));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(6988), new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(6992) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(6997), new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(6997) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(6999), new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(6999) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(7000), new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(7000) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(7001), new DateTime(2026, 5, 12, 20, 10, 37, 238, DateTimeKind.Utc).AddTicks(7001) });
        }
    }
}
