using System.ComponentModel.DataAnnotations;

namespace personelOtomasyon.Data.ViewModels
{
    public class DegerlendirmeRaporuVM
    {
        public int RaporId { get; set; }  

        [Required]
        public int BasvuruId { get; set; }

        [Required(ErrorMessage = "Sonuç seçimi zorunludur.")]
        public string Sonuc { get; set; }

        [Required(ErrorMessage = "Rapor metni zorunludur.")]
        public string RaporDosyasi { get; set; }
    }

}
