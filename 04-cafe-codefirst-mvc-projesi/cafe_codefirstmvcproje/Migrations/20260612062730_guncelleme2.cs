using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cafe_codefirstmvcproje.Migrations
{
    /// <inheritdoc />
    public partial class guncelleme2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Comments");
        }
    }
}
