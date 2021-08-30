using Microsoft.EntityFrameworkCore.Migrations;

namespace OnePennyCoin.Data.Migrations
{
    public partial class AddEBankingUserPOCO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionKey",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionKey",
                table: "AspNetUsers");
        }
    }
}
