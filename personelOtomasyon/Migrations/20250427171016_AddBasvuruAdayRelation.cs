using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace personelOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class AddBasvuruAdayRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //  Mevcut verileri yeni kolona taşı
            migrationBuilder.Sql(
                @"UPDATE Basvurular SET KullaniciAdayId = ApplicationUserId WHERE KullaniciAdayId IS NULL"
            );

            //  Eski foreign key ve indexleri kaldır
            migrationBuilder.DropForeignKey(
                name: "FK_Basvurular_AspNetUsers_ApplicationUserId",
                table: "Basvurular");

            migrationBuilder.DropIndex(
                name: "IX_Basvurular_ApplicationUserId",
                table: "Basvurular");

            //  Eski kolonu güvenle sil
            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Basvurular");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Basvurular",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Basvurular_ApplicationUserId",
                table: "Basvurular",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Basvurular_AspNetUsers_ApplicationUserId",
                table: "Basvurular",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
