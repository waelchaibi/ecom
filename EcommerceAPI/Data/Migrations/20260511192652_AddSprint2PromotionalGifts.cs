using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EcommerceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSprint2PromotionalGifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StockQuantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GiftRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RuleType = table.Column<int>(type: "integer", nullable: false),
                    ConditionValue = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GiftId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiftRules_Gifts_GiftId",
                        column: x => x.GiftId,
                        principalTable: "Gifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderGifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    GiftId = table.Column<int>(type: "integer", nullable: false),
                    GiftRuleId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderGifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderGifts_GiftRules_GiftRuleId",
                        column: x => x.GiftRuleId,
                        principalTable: "GiftRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderGifts_Gifts_GiftId",
                        column: x => x.GiftId,
                        principalTable: "Gifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderGifts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1362));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1369));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1370));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1046), new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1053) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1060), new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1060) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1061), new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1062) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1063), new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1063) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1064), new DateTime(2026, 5, 11, 19, 26, 52, 403, DateTimeKind.Utc).AddTicks(1064) });

            migrationBuilder.CreateIndex(
                name: "IX_GiftRules_GiftId",
                table: "GiftRules",
                column: "GiftId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGifts_GiftId",
                table: "OrderGifts",
                column: "GiftId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGifts_GiftRuleId",
                table: "OrderGifts",
                column: "GiftRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGifts_OrderId",
                table: "OrderGifts",
                column: "OrderId");

            migrationBuilder.InsertData(
                table: "Gifts",
                columns: new[] { "Id", "Name", "Description", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "Premium Gift Pack", "Complimentary pack when the cart total exceeds the amount threshold.", 250 },
                    { 2, "Loyalty Mug", "Thank-you gift for returning customers.", 200 },
                    { 3, "Promo Keychain", "Gift when promotion code SPRINT2 is applied.", 500 }
                });

            migrationBuilder.InsertData(
                table: "GiftRules",
                columns: new[] { "Id", "RuleType", "ConditionValue", "GiftId", "IsActive", "Priority" },
                values: new object[,]
                {
                    { 1, 0, "100", 1, true, 200 },
                    { 2, 1, "0", 2, true, 100 },
                    { 3, 2, "SPRINT2", 3, true, 150 }
                });

            migrationBuilder.Sql(
                "SELECT setval(pg_get_serial_sequence('\"Gifts\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Gifts\"), 1));");

            migrationBuilder.Sql(
                "SELECT setval(pg_get_serial_sequence('\"GiftRules\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"GiftRules\"), 1));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderGifts");

            migrationBuilder.DropTable(
                name: "GiftRules");

            migrationBuilder.DropTable(
                name: "Gifts");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8321));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8326));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8328));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8076), new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8079) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8086), new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8086) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8089), new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8089) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8091), new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8091) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8093), new DateTime(2026, 5, 4, 21, 5, 48, 159, DateTimeKind.Utc).AddTicks(8093) });
        }
    }
}
