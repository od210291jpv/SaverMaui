using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaverBackend.Db.Migrations
{
    public partial class AddCmsId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CmsExternalId",
                table: "Contents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CmsExternalId",
                table: "Contents");
        }
    }
}
