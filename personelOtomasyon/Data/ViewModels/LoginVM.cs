using System.ComponentModel.DataAnnotations;

namespace personelOtomasyon.Data.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email giriniz!")]
        public string EmailAddress { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre giriniz!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
