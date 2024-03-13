using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhiteLagoon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addVillaNumber2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VillaNumber_Villas_VillaId",
                table: "VillaNumber");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VillaNumber",
                table: "VillaNumber");

            migrationBuilder.RenameTable(
                name: "VillaNumber",
                newName: "VillaNumbers");

            migrationBuilder.RenameIndex(
                name: "IX_VillaNumber_VillaId",
                table: "VillaNumbers",
                newName: "IX_VillaNumbers_VillaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VillaNumbers",
                table: "VillaNumbers",
                column: "Villa_Number");

            migrationBuilder.AddForeignKey(
                name: "FK_VillaNumbers_Villas_VillaId",
                table: "VillaNumbers",
                column: "VillaId",
                principalTable: "Villas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VillaNumbers_Villas_VillaId",
                table: "VillaNumbers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VillaNumbers",
                table: "VillaNumbers");

            migrationBuilder.RenameTable(
                name: "VillaNumbers",
                newName: "VillaNumber");

            migrationBuilder.RenameIndex(
                name: "IX_VillaNumbers_VillaId",
                table: "VillaNumber",
                newName: "IX_VillaNumber_VillaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VillaNumber",
                table: "VillaNumber",
                column: "Villa_Number");

            migrationBuilder.AddForeignKey(
                name: "FK_VillaNumber_Villas_VillaId",
                table: "VillaNumber",
                column: "VillaId",
                principalTable: "Villas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
