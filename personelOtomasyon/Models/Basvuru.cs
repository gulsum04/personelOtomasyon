using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personelOtomasyon.Models
{
    public class Basvuru
    {
        [Key]
        public int BasvuruId { get; set; }

        public int? IlanId { get; set; }

        [ForeignKey("IlanId")]
        public AkademikIlan Ilan { get; set; }

        public string KullaniciAdayId { get; set; }

        [ForeignKey("KullaniciAdayId")]
    
        public virtual ApplicationUser Aday { get; set; }
        public DateTime BasvuruTarihi { get; set; }
        public string Durum { get; set; }

        public ICollection<BasvuruBelge> Belgeler { get; set; }
        public ICollection<DegerlendirmeRaporu> DegerlendirmeRaporlari { get; set; }

        public string? JuriSonucu { get; set; }
        public string? JuriRaporu { get; set; }
        public bool? DegerlendirmeTamamlandiMi { get; set; } = false;
        public int? ToplamPuan { get; set; }
        public string? YoneticiSonucu { get; set; }

        public ICollection<BasvuruPuan> BasvuruPuanlar { get; set; }



    }
}
