using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using personelOtomasyon.Models;

namespace personelOtomasyon.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<AkademikIlan> AkademikIlanlar { get; set; }
        public DbSet<BasvuruBelge> BasvuruBelgeleri { get; set; }
        public DbSet<Basvuru> Basvurular { get; set; }
        public DbSet<DegerlendirmeRaporu> DegerlendirmeRaporlari { get; set; }
        public DbSet<JuriUyesi> JuriUyeleri { get; set; }
        public DbSet<KadroKriteri> KadroKriterleri { get; set; }
        public DbSet<BasvuruJuri> BasvuruJuriAtamalari { get; set; }
        public DbSet<Bildirim> Bildirimler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aday - Başvuru ilişkisi (Restrict)
            modelBuilder.Entity<Basvuru>()
                .HasOne(b => b.Aday)
                .WithMany()
                .HasForeignKey(b => b.KullaniciAdayId)
                .OnDelete(DeleteBehavior.Restrict);

            // Admin - İlan ilişkisi (Restrict)
            modelBuilder.Entity<AkademikIlan>()
                .HasOne(i => i.Admin)
                .WithMany()
                .HasForeignKey(i => i.KullaniciAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Başvuru - Jüri ataması ilişkisi (Restrict)
            modelBuilder.Entity<BasvuruJuri>()
      .HasOne(bj => bj.Basvuru)
      .WithMany()
      .HasForeignKey(bj => bj.BasvuruId)
      .OnDelete(DeleteBehavior.Cascade); // 👈 BU ÖNEMLİ

            // ❗ Başvuru - Belge İlişkisi Cascade yapılmalı
            modelBuilder.Entity<BasvuruBelge>()
                .HasOne(bb => bb.Basvuru)
                .WithMany(b => b.Belgeler)
                .HasForeignKey(bb => bb.BasvuruId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BasvuruJuri>()
                .HasOne(bj => bj.Juri)
                .WithMany()
                .HasForeignKey(bj => bj.JuriId)
                .OnDelete(DeleteBehavior.Restrict);

            // İlan - Başvuru ilişkisi: ❗ İlan silinirse başvurular da silinsin
            modelBuilder.Entity<Basvuru>()
     .HasOne(b => b.Ilan)
     .WithMany(i => i.Basvurular) // ❗ burası çok önemli
     .HasForeignKey(b => b.IlanId)
     .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
