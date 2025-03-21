using System.ComponentModel.DataAnnotations;

namespace personelOtomasyon.Models
{
    public class Kullanici
    {
        [Key]
        public int KullaniciId { get; set; }
        public string TcKimlikNo { get; set; }
        public string AdSoyad { get; set; }
        public string Email { get; set; }
        public string SifreHash { get; set; }
        public int? RolId { get; set; }
        public DateTime KayitTarihi { get; set; }
    }
}
