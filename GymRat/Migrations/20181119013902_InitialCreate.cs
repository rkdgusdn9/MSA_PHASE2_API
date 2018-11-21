using Microsoft.EntityFrameworkCore.Migrations;

namespace GymRat.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GymItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    BigPart = table.Column<string>(nullable: true),
                    SmallPart = table.Column<string>(nullable: true),
                    Direction = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    Uploaded = table.Column<string>(nullable: true),
                    Width = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymItem", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GymItem");
        }
    }
}
