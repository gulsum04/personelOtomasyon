using System.ComponentModel.DataAnnotations;

namespace personelOtomasyon.Models
{
    public class JuriUyesi
    {
        [Key]
        public int JuriUyesiId { get; set; }
        public int IlanId { get; set; }
        public int KullaniciJuriId { get; set; }
    }
}
