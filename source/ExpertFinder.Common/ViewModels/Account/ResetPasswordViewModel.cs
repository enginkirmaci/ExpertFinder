using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        public string UserId { get; set; }

        public string Code { get; set; }

        [Required(ErrorMessage = "Şifrenizi girmelisiniz.")]
        [StringLength(100, ErrorMessage = "Şifreniz en az 6 haneli olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Onay")]
        [Compare("Password", ErrorMessage = "Şifre ve şifre onayı aynı olmalıdır.")]
        public string ConfirmPassword { get; set; }
    }
}