using EcommerceAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260723120000_AddProductIsActive")]
    public partial class AddProductIsActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER TABLE ""Products"" ADD COLUMN IF NOT EXISTS ""IsActive"" boolean NOT NULL DEFAULT TRUE;
CREATE INDEX IF NOT EXISTS ""IX_Products_IsActive"" ON ""Products"" (""IsActive"");
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP INDEX IF EXISTS ""IX_Products_IsActive"";
ALTER TABLE ""Products"" DROP COLUMN IF EXISTS ""IsActive"";
");
        }
    }
}
