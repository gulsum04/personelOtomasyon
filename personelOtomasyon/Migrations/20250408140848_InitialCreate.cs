using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace personelOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AkademikIlanlar",
                columns: table => new
                {
                    IlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemelAlan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BasvuruBaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BasvuruBitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KullaniciAdminId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Yayinda = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AkademikIlanlar", x => x.IlanId);
                    table.ForeignKey(
                        name: "FK_AkademikIlanlar_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AkademikIlanlar_AspNetUsers_KullaniciAdminId",
                        column: x => x.KullaniciAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bildirimler",
                columns: table => new
                {
                    BildirimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Mesaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OkunduMu = table.Column<bool>(type: "bit", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirimler", x => x.BildirimId);
                    table.ForeignKey(
                        name: "FK_Bildirimler_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Basvurular",
                columns: table => new
                {
                    BasvuruId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IlanId = table.Column<int>(type: "int", nullable: true),
                    KullaniciAdayId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BasvuruTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Basvurular", x => x.BasvuruId);
                    table.ForeignKey(
                        name: "FK_Basvurular_AkademikIlanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "AkademikIlanlar",
                        principalColumn: "IlanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Basvurular_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Basvurular_AspNetUsers_KullaniciAdayId",
                        column: x => x.KullaniciAdayId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JuriUyeleri",
                columns: table => new
                {
                    JuriUyesiId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IlanId = table.Column<int>(type: "int", nullable: false),
                    KullaniciJuriId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JuriUyeleri", x => x.JuriUyesiId);
                    table.ForeignKey(
                        name: "FK_JuriUyeleri_AkademikIlanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "AkademikIlanlar",
                        principalColumn: "IlanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JuriUyeleri_AspNetUsers_KullaniciJuriId",
                        column: x => x.KullaniciJuriId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KadroKriterleri",
                columns: table => new
                {
                    KriterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IlanId = table.Column<int>(type: "int", nullable: false),
                    KriterAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZorunluMu = table.Column<bool>(type: "bit", nullable: false),
                    BelgeYuklenecekMi = table.Column<bool>(type: "bit", nullable: false),
                    BelgeSayisi = table.Column<int>(type: "int", nullable: false),
                    TemelAlan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unvan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KullaniciYoneticiId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KadroKriterleri", x => x.KriterId);
                    table.ForeignKey(
                        name: "FK_KadroKriterleri_AkademikIlanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "AkademikIlanlar",
                        principalColumn: "IlanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KadroKriterleri_AspNetUsers_KullaniciYoneticiId",
                        column: x => x.KullaniciYoneticiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BasvuruBelgeleri",
                columns: table => new
                {
                    BasvuruBelgeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasvuruId = table.Column<int>(type: "int", nullable: false),
                    BelgeTuru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DosyaYolu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasvuruBelgeleri", x => x.BasvuruBelgeId);
                    table.ForeignKey(
                        name: "FK_BasvuruBelgeleri_Basvurular_BasvuruId",
                        column: x => x.BasvuruId,
                        principalTable: "Basvurular",
                        principalColumn: "BasvuruId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DegerlendirmeRaporlari",
                columns: table => new
                {
                    RaporId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciJuriId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BasvuruId = table.Column<int>(type: "int", nullable: false),
                    RaporDosyasi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sonuc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DegerlendirmeRaporlari", x => x.RaporId);
                    table.ForeignKey(
                        name: "FK_DegerlendirmeRaporlari_AspNetUsers_KullaniciJuriId",
                        column: x => x.KullaniciJuriId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DegerlendirmeRaporlari_Basvurular_BasvuruId",
                        column: x => x.BasvuruId,
                        principalTable: "Basvurular",
                        principalColumn: "BasvuruId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KadroKriterAltlar",
                columns: table => new
                {
                    AltId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KriterId = table.Column<int>(type: "int", nullable: false),
                    BelgeTuru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BelgeSayisi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KadroKriterAltlar", x => x.AltId);
                    table.ForeignKey(
                        name: "FK_KadroKriterAltlar_KadroKriterleri_KriterId",
                        column: x => x.KriterId,
                        principalTable: "KadroKriterleri",
                        principalColumn: "KriterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AkademikIlanlar_ApplicationUserId",
                table: "AkademikIlanlar",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AkademikIlanlar_KullaniciAdminId",
                table: "AkademikIlanlar",
                column: "KullaniciAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruBelgeleri_BasvuruId",
                table: "BasvuruBelgeleri",
                column: "BasvuruId");

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruJuriAtamalari_BasvuruId",
                table: "BasvuruJuriAtamalari",
                column: "BasvuruId");

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruJuriAtamalari_JuriId",
                table: "BasvuruJuriAtamalari",
                column: "JuriId");

            migrationBuilder.CreateIndex(
                name: "IX_Basvurular_ApplicationUserId",
                table: "Basvurular",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Basvurular_IlanId",
                table: "Basvurular",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Basvurular_KullaniciAdayId",
                table: "Basvurular",
                column: "KullaniciAdayId");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_KullaniciId",
                table: "Bildirimler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_DegerlendirmeRaporlari_BasvuruId",
                table: "DegerlendirmeRaporlari",
                column: "BasvuruId");

            migrationBuilder.CreateIndex(
                name: "IX_DegerlendirmeRaporlari_KullaniciJuriId",
                table: "DegerlendirmeRaporlari",
                column: "KullaniciJuriId");

            migrationBuilder.CreateIndex(
                name: "IX_JuriUyeleri_IlanId",
                table: "JuriUyeleri",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_JuriUyeleri_KullaniciJuriId",
                table: "JuriUyeleri",
                column: "KullaniciJuriId");

            migrationBuilder.CreateIndex(
                name: "IX_KadroKriterAltlar_KriterId",
                table: "KadroKriterAltlar",
                column: "KriterId");

            migrationBuilder.CreateIndex(
                name: "IX_KadroKriterleri_IlanId",
                table: "KadroKriterleri",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_KadroKriterleri_KullaniciYoneticiId",
                table: "KadroKriterleri",
                column: "KullaniciYoneticiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BasvuruBelgeleri");

            migrationBuilder.DropTable(
                name: "BasvuruJuriAtamalari");

            migrationBuilder.DropTable(
                name: "Bildirimler");

            migrationBuilder.DropTable(
                name: "DegerlendirmeRaporlari");

            migrationBuilder.DropTable(
                name: "JuriUyeleri");

            migrationBuilder.DropTable(
                name: "KadroKriterAltlar");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Basvurular");

            migrationBuilder.DropTable(
                name: "KadroKriterleri");

            migrationBuilder.DropTable(
                name: "AkademikIlanlar");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
