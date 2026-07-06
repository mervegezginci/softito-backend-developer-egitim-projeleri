using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cafe_codefirstmvcproje.Migrations
{
    /// <inheritdoc />
    public partial class guncelleme4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductNo",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductNo",
                table: "Categories");
        }
    }
}
