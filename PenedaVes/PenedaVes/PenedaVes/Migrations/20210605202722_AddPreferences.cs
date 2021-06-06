using Microsoft.EntityFrameworkCore.Migrations;

namespace PenedaVes.Migrations
{
    public partial class AddPreferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FollowedCamera",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CameraId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowedCamera", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowedCamera_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FollowedCamera_Camera_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Camera",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FollowedSpecies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpeciesId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowedSpecies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowedSpecies_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FollowedSpecies_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FollowedCamera_CameraId",
                table: "FollowedCamera",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowedCamera_UserId",
                table: "FollowedCamera",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowedSpecies_SpeciesId",
                table: "FollowedSpecies",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowedSpecies_UserId",
                table: "FollowedSpecies",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FollowedCamera");

            migrationBuilder.DropTable(
                name: "FollowedSpecies");
        }
    }
}
