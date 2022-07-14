using Microsoft.EntityFrameworkCore.Migrations;

namespace P127_Pronia.Migrations
{
    public partial class addIsMainToPlantImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "PlantImages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "PlantImages");
        }
    }
}
