using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personelOtomasyon.Models
{
    public class DegerlendirmeRaporu
    {
        [Key]
        public int RaporId { get; set; }
        public int KullaniciJuriId { get; set; }
        public int BasvuruId { get; set; }

        [ForeignKey("BasvuruId")]
        public Basvuru Basvuru { get; set; }
        public string RaporDosyasi { get; set; }
        public string Sonuc { get; set; }
    }
}
