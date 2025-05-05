using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personelOtomasyon.Models
{
    public class KadroKriteri
    {
        [Key]
        public int KriterId { get; set; }

        public int IlanId { get; set; }

        [ForeignKey("IlanId")]
        public AkademikIlan Ilan { get; set; }
        public string KriterAdi { get; set; }
        public string KullaniciYoneticiId { get; set; }

        [ForeignKey("KullaniciYoneticiId")]
        public ApplicationUser Yonetici { get; set; }
        public string Gereklilik { get; set; }
    }
}
