using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace personelOtomasyon.Models
{
    public class DegerlendirmeRaporu
    {
        [Key]
        public int RaporId { get; set; }

        [BindNever]
        public string KullaniciJuriId { get; set; }

        [ForeignKey("KullaniciJuriId")]
        [BindNever]
        public ApplicationUser Juri { get; set; }

        [BindNever]
        public int BasvuruId { get; set; }

        [ForeignKey("BasvuruId")]
        [BindNever]
        public Basvuru Basvuru { get; set; }

        [Required(ErrorMessage = "Rapor metni zorunludur.")]
        public string RaporDosyasi { get; set; }

        [Required(ErrorMessage = "Sonuç seçimi zorunludur.")]
        public string Sonuc { get; set; }
    }
}
