using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using personelOtomasyon.Models;

namespace personelOtomasyon.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AkademikIlan> AkademikIlanlar { get; set; }
        public DbSet<BasvuruBelge> BasvuruBelgeleri { get; set; }
        public DbSet<Basvuru> Basvurular { get; set; }
        public DbSet<DegerlendirmeRaporu> DegerlendirmeRaporlari { get; set; }
        public DbSet<JuriUyesi> JuriUyeleri { get; set; }
        public DbSet<KadroKriteri> KadroKriterleri { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // İlişkilerde silme durumunda çakışmayı engelle
            modelBuilder.Entity<Basvuru>()
                .HasOne(b => b.Aday)
                .WithMany()
                .HasForeignKey(b => b.KullaniciAdayId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AkademikIlan>()
                .HasOne(i => i.Admin)
                .WithMany()
                .HasForeignKey(i => i.KullaniciAdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
