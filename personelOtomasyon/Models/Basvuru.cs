using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personelOtomasyon.Models
{
    public class Basvuru
    {
        [Key]
        public int BasvuruId { get; set; }

        public int IlanId { get; set; }

        [ForeignKey("IlanId")]
        public AkademikIlan Ilan { get; set; }

        public string KullaniciAdayId { get; set; }

        [ForeignKey("KullaniciAdayId")]
        public ApplicationUser Aday { get; set; }

        public DateTime BasvuruTarihi { get; set; }
        public string Durum { get; set; }

        public ICollection<BasvuruBelge> Belgeler { get; set; }
    }
}
