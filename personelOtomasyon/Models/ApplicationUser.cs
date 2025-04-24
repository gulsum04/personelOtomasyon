using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace personelOtomasyon.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; }

        [Required]
        public string TcKimlikNo { get; set; }

        public int DogumYili { get; set; }

        public DateTime KayitTarihi { get; set; }
        public ICollection<Basvuru> Basvurular { get; set; }
        public ICollection<AkademikIlan> Ilanlar { get; set; }
    }
}
