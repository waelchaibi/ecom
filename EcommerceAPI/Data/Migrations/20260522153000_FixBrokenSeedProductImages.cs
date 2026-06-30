using EcommerceAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260522153000_FixBrokenSeedProductImages")]
    /// <inheritdoc />
    public partial class FixBrokenSeedProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $"UPDATE \"Products\" SET \"ImageUrl\" = '{SeedProductImages.Laptop}' WHERE \"Id\" = 1;");
            migrationBuilder.Sql(
                $"UPDATE \"Products\" SET \"ImageUrl\" = '{SeedProductImages.Mouse}' WHERE \"Id\" = 2;");
            migrationBuilder.Sql(
                $"UPDATE \"Products\" SET \"ImageUrl\" = '{SeedProductImages.Keyboard}' WHERE \"Id\" = 3;");
            migrationBuilder.Sql(
                $"UPDATE \"Products\" SET \"ImageUrl\" = '{SeedProductImages.Monitor}' WHERE \"Id\" = 4;");
            migrationBuilder.Sql(
                $"UPDATE \"Products\" SET \"ImageUrl\" = '{SeedProductImages.UsbCable}' WHERE \"Id\" = 5;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
