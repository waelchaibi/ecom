using EcommerceAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260719193000_AddChatTables")]
    public partial class AddChatTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS ""ChatThreads"" (
    ""Id"" serial PRIMARY KEY,
    ""UserRole"" character varying(20) NOT NULL,
    ""CustomerId"" integer NULL REFERENCES ""Customers"" (""Id"") ON DELETE CASCADE,
    ""AdminUsername"" character varying(100) NULL,
    ""Title"" character varying(200) NOT NULL DEFAULT 'New chat',
    ""CreatedAt"" timestamp with time zone NOT NULL,
    ""UpdatedAt"" timestamp with time zone NOT NULL
);

CREATE INDEX IF NOT EXISTS ""IX_ChatThreads_CustomerId"" ON ""ChatThreads"" (""CustomerId"");
CREATE INDEX IF NOT EXISTS ""IX_ChatThreads_AdminUsername"" ON ""ChatThreads"" (""AdminUsername"");

CREATE TABLE IF NOT EXISTS ""ChatMessages"" (
    ""Id"" serial PRIMARY KEY,
    ""ThreadId"" integer NOT NULL REFERENCES ""ChatThreads"" (""Id"") ON DELETE CASCADE,
    ""Role"" character varying(20) NOT NULL,
    ""Content"" text NOT NULL,
    ""CreatedAt"" timestamp with time zone NOT NULL
);

CREATE INDEX IF NOT EXISTS ""IX_ChatMessages_ThreadId"" ON ""ChatMessages"" (""ThreadId"");
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP TABLE IF EXISTS ""ChatMessages"";
DROP TABLE IF EXISTS ""ChatThreads"";
");
        }
    }
}
