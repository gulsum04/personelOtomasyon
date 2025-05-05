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
        public DbSet<KadroKriterAlt> KadroKriterAltlar { get; set; }
        public DbSet<BasvuruPuan> BasvuruPuanlar { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        
        {
            base.OnModelCreating(modelBuilder);

            // Aday - Başvuru ilişkisi (Restrict)
            modelBuilder.Entity<Basvuru>()
                .HasOne(b => b.Aday)
                .WithMany(u => u.Basvurular)
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
      .OnDelete(DeleteBehavior.Cascade); 

            
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
     .WithMany(i => i.Basvurular) 
     .HasForeignKey(b => b.IlanId)
     .OnDelete(DeleteBehavior.Cascade);

            // İlan - KadroKriteri ilişkisi
            modelBuilder.Entity<KadroKriteri>()
                .HasOne(k => k.Ilan)
                .WithMany(i => i.KadroKriterleri)
                .HasForeignKey(k => k.IlanId)
                .OnDelete(DeleteBehavior.Cascade); // İlan silinirse kriterleri de silinsin


            modelBuilder.Entity<BasvuruPuan>()
    .HasOne(p => p.Basvuru)
    .WithMany(b => b.BasvuruPuanlar)
    .HasForeignKey(p => p.BasvuruId)
    .OnDelete(DeleteBehavior.Cascade);


        }



    }
}
