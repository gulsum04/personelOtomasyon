using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace personelOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class JuriIdDegisiklik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Önce mevcut PK'yı kaldır
            migrationBuilder.DropPrimaryKey(
                name: "PK_JuriUyeleri",
                table: "JuriUyeleri");

            // 2. Eski IDENTITY olan kolonu sil
            migrationBuilder.DropColumn(
                name: "JuriUyesiId",
                table: "JuriUyeleri");

            // 3. Yeni string kolonu ekle
            migrationBuilder.AddColumn<string>(
                name: "JuriUyesiId",
                table: "JuriUyeleri",
                type: "nvarchar(450)",
                nullable: false,
                defaultValueSql: "NEWID()");

            // 4. Yeni kolonu tekrar PK olarak ata
            migrationBuilder.AddPrimaryKey(
                name: "PK_JuriUyeleri",
                table: "JuriUyeleri",
                column: "JuriUyesiId");
        }



        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JuriUyeleri",
                table: "JuriUyeleri");

            migrationBuilder.DropColumn(
                name: "JuriUyesiId",
                table: "JuriUyeleri");

            migrationBuilder.AddColumn<int>(
                name: "JuriUyesiId",
                table: "JuriUyeleri",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JuriUyeleri",
                table: "JuriUyeleri",
                column: "JuriUyesiId");
        }
    }
}
