using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce532.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentPricePropToCartModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentPrice",
                table: "Carts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPrice",
                table: "Carts");
        }
    }
}
