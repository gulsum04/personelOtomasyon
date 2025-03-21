using System.ComponentModel.DataAnnotations;

namespace personelOtomasyon.Data.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Ad Soyad giriniz!")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email giriniz!")]
        public string EmailAddress { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre giriniz!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Şifre doğrulama")]
        [Required(ErrorMessage = "Şifreyi doğrulayınız!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor!")]
        public string ConfirmPassword { get; set; }
    }
}
