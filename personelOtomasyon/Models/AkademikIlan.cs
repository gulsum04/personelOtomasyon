using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace personelOtomasyon.Models
{
    public class AkademikIlan
    {
        [Key]
        public int IlanId { get; set; }

        [Required]
        public string Baslik { get; set; }

        [Required]
        public string Kategori { get; set; }

        [Required]
        public string Aciklama { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BasvuruBaslangicTarihi { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BasvuruBitisTarihi { get; set; }

        [ValidateNever] // ❗ Kullanıcıdan gelmeyecek
        public string KullaniciAdminId { get; set; }

        [ValidateNever] // ❗ Navigation property
        [ForeignKey("KullaniciAdminId")]
        public ApplicationUser Admin { get; set; }

        [ValidateNever] // ❗ Formdan gelmez
        public ICollection<Basvuru> Basvurular { get; set; }
    }
}
