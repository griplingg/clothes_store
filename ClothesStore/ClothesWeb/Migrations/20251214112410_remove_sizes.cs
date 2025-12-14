using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothesWeb.Migrations
{
    /// <inheritdoc />
    public partial class remove_sizes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sizes",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sizes",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
