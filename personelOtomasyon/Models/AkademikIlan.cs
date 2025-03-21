using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personelOtomasyon.Models
{
    public class AkademikIlan
    {
        [Key]
        public int IlanId { get; set; }

        public string Baslik { get; set; }
        public string Kategori { get; set; }
        public string Aciklama { get; set; }
        public DateTime BasvuruBaslangicTarihi { get; set; }
        public DateTime BasvuruBitisTarihi { get; set; }

        public string KullaniciAdminId { get; set; }

        [ForeignKey("KullaniciAdminId")]
        public ApplicationUser Admin { get; set; }

        public ICollection<Basvuru> Basvurular { get; set; }
    }
}
