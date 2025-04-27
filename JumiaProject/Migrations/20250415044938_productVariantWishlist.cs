using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JumiaProject.Migrations
{
    /// <inheritdoc />
    public partial class productVariantWishlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "Wishlists",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_ProductVariantId",
                table: "Wishlists",
                column: "ProductVariantId",
                unique: true,
                filter: "[ProductVariantId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK__Wishlist__ProductVariant",
                table: "Wishlists",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "VariantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Wishlist__ProductVariant",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_ProductVariantId",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "Wishlists");

        }
    }
}
