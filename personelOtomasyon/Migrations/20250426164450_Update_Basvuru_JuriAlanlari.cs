using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace personelOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class Update_Basvuru_JuriAlanlari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DegerlendirmeTamamlandiMi",
                table: "Basvurular",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JuriRaporu",
                table: "Basvurular",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JuriSonucu",
                table: "Basvurular",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ToplamPuan",
                table: "Basvurular",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegerlendirmeTamamlandiMi",
                table: "Basvurular");

            migrationBuilder.DropColumn(
                name: "JuriRaporu",
                table: "Basvurular");

            migrationBuilder.DropColumn(
                name: "JuriSonucu",
                table: "Basvurular");

            migrationBuilder.DropColumn(
                name: "ToplamPuan",
                table: "Basvurular");
        }
    }
}
