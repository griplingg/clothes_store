using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothesWeb.Migrations
{
    /// <inheritdoc />
    public partial class updateReturnsLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReturnItems");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnProducts_SellItemId",
                table: "ReturnProducts",
                column: "SellItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnProducts_SellItems_SellItemId",
                table: "ReturnProducts",
                column: "SellItemId",
                principalTable: "SellItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnProducts_SellItems_SellItemId",
                table: "ReturnProducts");

            migrationBuilder.DropIndex(
                name: "IX_ReturnProducts_SellItemId",
                table: "ReturnProducts");

            migrationBuilder.CreateTable(
                name: "ReturnItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnId = table.Column<int>(type: "int", nullable: false),
                    SellItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnItems_ReturnProducts_ReturnId",
                        column: x => x.ReturnId,
                        principalTable: "ReturnProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReturnItems_SellItems_SellItemId",
                        column: x => x.SellItemId,
                        principalTable: "SellItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_ReturnId",
                table: "ReturnItems",
                column: "ReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_SellItemId",
                table: "ReturnItems",
                column: "SellItemId");
        }
    }
}
