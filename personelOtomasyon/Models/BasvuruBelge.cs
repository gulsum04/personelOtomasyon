using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personelOtomasyon.Models
{
    public class BasvuruBelge
    {
        [Key]
        public int BasvuruBelgeId { get; set; }

        public int BasvuruId { get; set; }

        [ForeignKey("BasvuruId")]
        public Basvuru Basvuru { get; set; }

        public string BelgeTuru { get; set; }
        public string DosyaYolu { get; set; }
    }
}
