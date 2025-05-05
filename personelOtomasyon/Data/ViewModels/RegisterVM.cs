using System.ComponentModel.DataAnnotations;

namespace personelOtomasyon.Data.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Ad Soyad giriniz!")]
        public string FullName { get; set; }

        [Display(Name = "TC Kimlik Numarası")]
        [Required(ErrorMessage = "TC Kimlik numarası giriniz!")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik numarası 11 haneli olmalıdır!")]
        [RegularExpression(@"^[1-9][0-9]{10}$", ErrorMessage = "Geçerli bir TC Kimlik numarası giriniz!")]
        public string TcKimlikNo { get; set; }

        [Display(Name = "Doğum Yılı")]
        [Required(ErrorMessage = "Doğum yılı giriniz!")]
        [Range(1900, 2100, ErrorMessage = "Geçerli bir yıl giriniz!")]
        public int DogumYili { get; set; }

        [Display(Name = "E-Posta Adresi")]
        [Required(ErrorMessage = "E-Posta adresi giriniz!")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz!")]
        public string Email { get; set; }

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
