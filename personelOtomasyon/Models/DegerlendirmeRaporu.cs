using System.ComponentModel.DataAnnotations;

namespace personelOtomasyon.Models
{
    public class DegerlendirmeRaporu
    {
        [Key]
        public int RaporId { get; set; }
        public int KullaniciJuriId { get; set; }
        public int BasvuruId { get; set; }
        public string RaporDosyasi { get; set; }
        public string Sonuc { get; set; }
    }
}
