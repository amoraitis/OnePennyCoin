using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnePennyCoin.Data.Migrations
{
    public partial class Add_Flow_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flow",
                columns: table => new
                {
                    FlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CardId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastRefreshDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flow", x => x.FlowId);
                    table.ForeignKey(
                        name: "FK_Flow_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flow_UserId",
                table: "Flow",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flow");
        }
    }
}
