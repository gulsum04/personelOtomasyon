using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personelOtomasyon.Models
{
    public class BasvuruPuan
    {
        [Key]
        public int BasvuruPuanId { get; set; }

        public int BasvuruId { get; set; }
        [ForeignKey("BasvuruId")]
        public Basvuru Basvuru { get; set; }

        [Required]
        public string BelgeTuru { get; set; } // Adayın yüklediği belge türü

        public string FaaliyetAdi { get; set; } // Yönergeye göre açıklama

        [Required]
        public int Puan { get; set; } // Verilen puan

        public string YoneticiId { get; set; }
        [ForeignKey("YoneticiId")]
        public ApplicationUser Yonetici { get; set; }

        public DateTime PuanVerilmeTarihi { get; set; } = DateTime.Now;
    }
}
