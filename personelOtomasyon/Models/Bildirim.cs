using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personelOtomasyon.Models
{
    public class Bildirim
    {
        [Key]
        public int BildirimId { get; set; }

        [Required]
        public string KullaniciId { get; set; }

        [ForeignKey("KullaniciId")]
        public ApplicationUser Kullanici { get; set; }

        [Required]
        public string Mesaj { get; set; }

        public bool OkunduMu { get; set; } = false;

        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}
