using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace personelOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Roller");

            migrationBuilder.AlterColumn<string>(
                name: "KullaniciYoneticiId",
                table: "KadroKriterleri",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "KullaniciJuriId",
                table: "JuriUyeleri",
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

            migrationBuilder.AddColumn<DateTime>(
                name: "KayitTarihi",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_KadroKriterleri_IlanId",
                table: "KadroKriterleri",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_KadroKriterleri_KullaniciYoneticiId",
                table: "KadroKriterleri",
                column: "KullaniciYoneticiId");

            migrationBuilder.CreateIndex(
                name: "IX_JuriUyeleri_IlanId",
                table: "JuriUyeleri",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_JuriUyeleri_KullaniciJuriId",
                table: "JuriUyeleri",
                column: "KullaniciJuriId");

            migrationBuilder.CreateIndex(
                name: "IX_DegerlendirmeRaporlari_BasvuruId",
                table: "DegerlendirmeRaporlari",
                column: "BasvuruId");

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
                name: "FK_DegerlendirmeRaporlari_Basvurular_BasvuruId",
                table: "DegerlendirmeRaporlari",
                column: "BasvuruId",
                principalTable: "Basvurular",
                principalColumn: "BasvuruId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JuriUyeleri_AkademikIlanlar_IlanId",
                table: "JuriUyeleri",
                column: "IlanId",
                principalTable: "AkademikIlanlar",
                principalColumn: "IlanId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JuriUyeleri_AspNetUsers_KullaniciJuriId",
                table: "JuriUyeleri",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DegerlendirmeRaporlari_AspNetUsers_KullaniciJuriId",
                table: "DegerlendirmeRaporlari");

            migrationBuilder.DropForeignKey(
                name: "FK_DegerlendirmeRaporlari_Basvurular_BasvuruId",
                table: "DegerlendirmeRaporlari");

            migrationBuilder.DropForeignKey(
                name: "FK_JuriUyeleri_AkademikIlanlar_IlanId",
                table: "JuriUyeleri");

            migrationBuilder.DropForeignKey(
                name: "FK_JuriUyeleri_AspNetUsers_KullaniciJuriId",
                table: "JuriUyeleri");

            migrationBuilder.DropForeignKey(
                name: "FK_KadroKriterleri_AkademikIlanlar_IlanId",
                table: "KadroKriterleri");

            migrationBuilder.DropForeignKey(
                name: "FK_KadroKriterleri_AspNetUsers_KullaniciYoneticiId",
                table: "KadroKriterleri");

            migrationBuilder.DropIndex(
                name: "IX_KadroKriterleri_IlanId",
                table: "KadroKriterleri");

            migrationBuilder.DropIndex(
                name: "IX_KadroKriterleri_KullaniciYoneticiId",
                table: "KadroKriterleri");

            migrationBuilder.DropIndex(
                name: "IX_JuriUyeleri_IlanId",
                table: "JuriUyeleri");

            migrationBuilder.DropIndex(
                name: "IX_JuriUyeleri_KullaniciJuriId",
                table: "JuriUyeleri");

            migrationBuilder.DropIndex(
                name: "IX_DegerlendirmeRaporlari_BasvuruId",
                table: "DegerlendirmeRaporlari");

            migrationBuilder.DropIndex(
                name: "IX_DegerlendirmeRaporlari_KullaniciJuriId",
                table: "DegerlendirmeRaporlari");

            migrationBuilder.DropColumn(
                name: "KayitTarihi",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "KullaniciYoneticiId",
                table: "KadroKriterleri",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "KullaniciJuriId",
                table: "JuriUyeleri",
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
                name: "Kullanicilar",
                columns: table => new
                {
                    KullaniciId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: true),
                    SifreHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.KullaniciId);
                });

            migrationBuilder.CreateTable(
                name: "Roller",
                columns: table => new
                {
                    RolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolAdi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roller", x => x.RolId);
                });
        }
    }
}
