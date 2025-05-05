using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace personelOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFaaliyetKoduFromBasvuruPuan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaaliyetKodu",
                table: "BasvuruPuanlar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FaaliyetKodu",
                table: "BasvuruPuanlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
