using EcommerceAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260522120000_UpdateSeedProductImages")]
    /// <inheritdoc />
    public partial class UpdateSeedProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"Products\" SET \"ImageUrl\" = 'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400&h=300&fit=crop&auto=format' WHERE \"Id\" = 1;");
            migrationBuilder.Sql(
                "UPDATE \"Products\" SET \"ImageUrl\" = 'https://images.unsplash.com/photo-1615661244837-44754495be92?w=400&h=300&fit=crop&auto=format' WHERE \"Id\" = 2;");
            migrationBuilder.Sql(
                "UPDATE \"Products\" SET \"ImageUrl\" = 'https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=400&h=300&fit=crop&auto=format' WHERE \"Id\" = 3;");
            migrationBuilder.Sql(
                "UPDATE \"Products\" SET \"ImageUrl\" = 'https://images.unsplash.com/photo-1593640408188-687383fc3309?w=400&h=300&fit=crop&auto=format' WHERE \"Id\" = 4;");
            migrationBuilder.Sql(
                "UPDATE \"Products\" SET \"ImageUrl\" = 'https://images.unsplash.com/photo-1558618666-fcd25c85f82e?w=400&h=300&fit=crop&auto=format' WHERE \"Id\" = 5;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"Products\" SET \"ImageUrl\" = 'https://picsum.photos/seed/laptop/400/300' WHERE \"Id\" = 1;");
            migrationBuilder.Sql(
                "UPDATE \"Products\" SET \"ImageUrl\" = 'https://picsum.photos/seed/mouse/400/300' WHERE \"Id\" = 2;");
            migrationBuilder.Sql(
                "UPDATE \"Products\" SET \"ImageUrl\" = 'https://picsum.photos/seed/keyboard/400/300' WHERE \"Id\" = 3;");
            migrationBuilder.Sql(
                "UPDATE \"Products\" SET \"ImageUrl\" = 'https://picsum.photos/seed/monitor/400/300' WHERE \"Id\" = 4;");
            migrationBuilder.Sql(
                "UPDATE \"Products\" SET \"ImageUrl\" = 'https://picsum.photos/seed/cable/400/300' WHERE \"Id\" = 5;");
        }
    }
}
