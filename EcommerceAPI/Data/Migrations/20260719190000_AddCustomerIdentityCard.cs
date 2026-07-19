using EcommerceAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260719190000_AddCustomerIdentityCard")]
    public partial class AddCustomerIdentityCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER TABLE ""Customers"" ADD COLUMN IF NOT EXISTS ""IdentityCard"" character varying(8) NULL;
UPDATE ""Customers""
SET ""IdentityCard"" = '9' || LPAD(""Id""::text, 7, '0')
WHERE ""IdentityCard"" IS NULL OR ""IdentityCard"" = '';
ALTER TABLE ""Customers"" ALTER COLUMN ""IdentityCard"" SET NOT NULL;
CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Customers_IdentityCard"" ON ""Customers"" (""IdentityCard"");
");

            migrationBuilder.Sql(@"
UPDATE ""Customers"" SET ""IdentityCard"" = '90000001' WHERE ""Id"" = 1;
UPDATE ""Customers"" SET ""IdentityCard"" = '90000002' WHERE ""Id"" = 2;
UPDATE ""Customers"" SET ""IdentityCard"" = '90000003' WHERE ""Id"" = 3;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP INDEX IF EXISTS ""IX_Customers_IdentityCard"";
ALTER TABLE ""Customers"" DROP COLUMN IF EXISTS ""IdentityCard"";
");
        }
    }
}
