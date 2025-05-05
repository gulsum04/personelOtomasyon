using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personelOtomasyon.Models
{
    public class KadroKriterAlt
    {
        [Key]
        public int AltId { get; set; }

        public int KriterId { get; set; }

        [ForeignKey("KriterId")]
        public KadroKriteri Kriter { get; set; }

        public string BelgeTuru { get; set; }
        public int BelgeSayisi { get; set; }


    }
}
