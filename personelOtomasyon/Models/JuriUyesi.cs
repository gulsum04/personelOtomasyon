using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personelOtomasyon.Models
{
    public class JuriUyesi
    {
        [Key]
        public string JuriUyesiId { get; set; } = Guid.NewGuid().ToString(); // 🟢 Otomatik ID
        public int IlanId { get; set; }

        [ForeignKey("IlanId")]
        public AkademikIlan Ilan { get; set; }
        public string KullaniciJuriId { get; set; }

        [ForeignKey("KullaniciJuriId")]
        public ApplicationUser Juri { get; set; }

    }
}
