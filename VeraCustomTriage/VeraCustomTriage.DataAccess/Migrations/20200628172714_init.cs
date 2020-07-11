using Microsoft.EntityFrameworkCore.Migrations;

namespace VeraCustomTriage.DataAccess.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutoResponse",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Response = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoResponse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyCondition",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Property = table.Column<string>(nullable: true),
                    Condition = table.Column<string>(nullable: true),
                    AutoResponseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyCondition_AutoResponse_AutoResponseId",
                        column: x => x.AutoResponseId,
                        principalTable: "AutoResponse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCondition_AutoResponseId",
                table: "PropertyCondition",
                column: "AutoResponseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyCondition");

            migrationBuilder.DropTable(
                name: "AutoResponse");
        }
    }
}
