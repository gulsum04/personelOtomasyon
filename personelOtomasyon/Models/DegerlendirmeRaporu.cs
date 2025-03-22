using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace personelOtomasyon.Models
{
    public class DegerlendirmeRaporu
    {
        [Key]
        public int RaporId { get; set; }
        public string KullaniciJuriId { get; set; }

        [ForeignKey("KullaniciJuriId")]
        public ApplicationUser Juri { get; set; }
        public int BasvuruId { get; set; }
        public string RaporDosyasi { get; set; }
        public string Sonuc { get; set; }
    }
}
