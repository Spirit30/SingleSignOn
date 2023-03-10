using Microsoft.EntityFrameworkCore.Migrations;

namespace SingleSignOn.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    ThirdPartyId = table.Column<string>(nullable: true),
                    AccessToken = table.Column<string>(nullable: true),
                    BearerToken = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    CodeTimestamp = table.Column<long>(nullable: false),
                    AccessTokenTimestamp = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_DisplayName_AccessToken_BearerToken",
                table: "Users",
                columns: new[] { "Email", "DisplayName", "AccessToken", "BearerToken" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
