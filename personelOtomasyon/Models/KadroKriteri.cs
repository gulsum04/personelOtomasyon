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
        public string Aciklama { get; set; }
        public bool ZorunluMu { get; set; }
        public bool BelgeYuklenecekMi { get; set; }
        public int BelgeSayisi { get; set; }

        public string TemelAlan { get; set; }
        public string Unvan { get; set; }

        public string KullaniciYoneticiId { get; set; }

        [ForeignKey("KullaniciYoneticiId")]
        public ApplicationUser Yonetici { get; set; }

        public ICollection<KadroKriterAlt> AltBelgeTurleri { get; set; }
    }
}
