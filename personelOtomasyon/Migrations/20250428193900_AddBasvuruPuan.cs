using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace personelOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class AddBasvuruPuan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BasvuruPuanlar",
                columns: table => new
                {
                    BasvuruPuanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasvuruId = table.Column<int>(type: "int", nullable: false),
                    BelgeTuru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FaaliyetKodu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FaaliyetAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Puan = table.Column<int>(type: "int", nullable: false),
                    YoneticiId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PuanVerilmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasvuruPuanlar", x => x.BasvuruPuanId);
                    table.ForeignKey(
                        name: "FK_BasvuruPuanlar_AspNetUsers_YoneticiId",
                        column: x => x.YoneticiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasvuruPuanlar_Basvurular_BasvuruId",
                        column: x => x.BasvuruId,
                        principalTable: "Basvurular",
                        principalColumn: "BasvuruId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruPuanlar_BasvuruId",
                table: "BasvuruPuanlar",
                column: "BasvuruId");

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruPuanlar_YoneticiId",
                table: "BasvuruPuanlar",
                column: "YoneticiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasvuruPuanlar");
        }
    }
}
