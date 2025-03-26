using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace personelOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class AddBasvuruJuriTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DegerlendirmeRaporlari_AspNetUsers_KullaniciJuriId",
                table: "DegerlendirmeRaporlari");

            migrationBuilder.DropForeignKey(
                name: "FK_KadroKriterleri_AspNetUsers_KullaniciYoneticiId",
                table: "KadroKriterleri");


            migrationBuilder.DropIndex(
                name: "IX_KadroKriterleri_KullaniciYoneticiId",
                table: "KadroKriterleri");

            migrationBuilder.DropIndex(
                name: "IX_DegerlendirmeRaporlari_KullaniciJuriId",
                table: "DegerlendirmeRaporlari");

            migrationBuilder.AlterColumn<int>(
                name: "KullaniciYoneticiId",
                table: "KadroKriterleri",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "KullaniciJuriId",
                table: "DegerlendirmeRaporlari",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "BasvuruJuriAtamalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasvuruId = table.Column<int>(type: "int", nullable: false),
                    JuriId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasvuruJuriAtamalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasvuruJuriAtamalari_AspNetUsers_JuriId",
                        column: x => x.JuriId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BasvuruJuriAtamalari_Basvurular_BasvuruId",
                        column: x => x.BasvuruId,
                        principalTable: "Basvurular",
                        principalColumn: "BasvuruId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruJuriAtamalari_BasvuruId",
                table: "BasvuruJuriAtamalari",
                column: "BasvuruId");

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruJuriAtamalari_JuriId",
                table: "BasvuruJuriAtamalari",
                column: "JuriId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasvuruJuriAtamalari");

            migrationBuilder.AlterColumn<string>(
                name: "KullaniciYoneticiId",
                table: "KadroKriterleri",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "KullaniciJuriId",
                table: "DegerlendirmeRaporlari",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_KadroKriterleri_IlanId",
                table: "KadroKriterleri",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_KadroKriterleri_KullaniciYoneticiId",
                table: "KadroKriterleri",
                column: "KullaniciYoneticiId");

            migrationBuilder.CreateIndex(
                name: "IX_DegerlendirmeRaporlari_KullaniciJuriId",
                table: "DegerlendirmeRaporlari",
                column: "KullaniciJuriId");

            migrationBuilder.AddForeignKey(
                name: "FK_DegerlendirmeRaporlari_AspNetUsers_KullaniciJuriId",
                table: "DegerlendirmeRaporlari",
                column: "KullaniciJuriId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KadroKriterleri_AkademikIlanlar_IlanId",
                table: "KadroKriterleri",
                column: "IlanId",
                principalTable: "AkademikIlanlar",
                principalColumn: "IlanId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KadroKriterleri_AspNetUsers_KullaniciYoneticiId",
                table: "KadroKriterleri",
                column: "KullaniciYoneticiId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
