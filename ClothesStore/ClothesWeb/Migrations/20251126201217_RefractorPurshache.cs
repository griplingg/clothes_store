using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothesWeb.Migrations
{
    /// <inheritdoc />
    public partial class RefractorPurshache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SellCompositions");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "SellItems");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "SellItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SellId",
                table: "SellItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "SellItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SellItems_ProductId",
                table: "SellItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SellItems_SellId",
                table: "SellItems",
                column: "SellId");

            migrationBuilder.CreateIndex(
                name: "IX_SellItems_SizeId",
                table: "SellItems",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SellItems_Products_ProductId",
                table: "SellItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SellItems_Sells_SellId",
                table: "SellItems",
                column: "SellId",
                principalTable: "Sells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SellItems_Sizes_SizeId",
                table: "SellItems",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellItems_Products_ProductId",
                table: "SellItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SellItems_Sells_SellId",
                table: "SellItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SellItems_Sizes_SizeId",
                table: "SellItems");

            migrationBuilder.DropIndex(
                name: "IX_SellItems_ProductId",
                table: "SellItems");

            migrationBuilder.DropIndex(
                name: "IX_SellItems_SellId",
                table: "SellItems");

            migrationBuilder.DropIndex(
                name: "IX_SellItems_SizeId",
                table: "SellItems");

            migrationBuilder.DropColumn(
                name: "SellId",
                table: "SellItems");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "SellItems");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "SellItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "SellItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SellCompositions",
                columns: table => new
                {
                    SellId = table.Column<int>(type: "int", nullable: false),
                    SellItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellCompositions", x => new { x.SellId, x.SellItemId });
                    table.ForeignKey(
                        name: "FK_SellCompositions_SellItems_SellItemId",
                        column: x => x.SellItemId,
                        principalTable: "SellItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellCompositions_Sells_SellId",
                        column: x => x.SellId,
                        principalTable: "Sells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SellCompositions_SellItemId",
                table: "SellCompositions",
                column: "SellItemId");
        }
    }
}
