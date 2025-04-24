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
        public string TemelAlan { get; set; } // ❗ Yeni eklenen alan

        [Required]
        [DataType(DataType.Date)]
        public DateTime BasvuruBaslangicTarihi { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BasvuruBitisTarihi { get; set; }

        [ValidateNever]
        public string KullaniciAdminId { get; set; }

        [ForeignKey("KullaniciAdminId")]
        [ValidateNever]
        public ApplicationUser Admin { get; set; }

        [ValidateNever]
        public bool Yayinda { get; set; } = false;

        [ValidateNever]
        public ICollection<KadroKriteri> KadroKriterleri { get; set; }

        [ValidateNever]
        public ICollection<Basvuru> Basvurular { get; set; }

    }

}
