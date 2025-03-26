using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personelOtomasyon.Models
{
    public class KadroKriteri
    {
        [Key]
        public int KriterId { get; set; }
        public int IlanId { get; set; }
        public string KriterAdi { get; set; }
        public int KullaniciYoneticiId { get; set; }
        public string Gereklilik { get; set; }
    }
}
