using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-mail adresinizi girmelisiniz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-mail adresi girmelisiniz!")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifrenizi girmelisiniz.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Beni hatırla?")]
        public bool RememberMe { get; set; }
    }
}