using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eticaret.Migrations
{
    /// <inheritdoc />
    public partial class TurEklendiYeni : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TurId",
                table: "Albums",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Albums_TurId",
                table: "Albums",
                column: "TurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_Turs_TurId",
                table: "Albums",
                column: "TurId",
                principalTable: "Turs",
                principalColumn: "TurId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_Turs_TurId",
                table: "Albums");

            migrationBuilder.DropIndex(
                name: "IX_Albums_TurId",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "TurId",
                table: "Albums");
        }
    }
}
