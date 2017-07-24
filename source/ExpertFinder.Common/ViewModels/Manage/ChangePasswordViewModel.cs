using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.ViewModels.Manage
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Eski Şifre")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Şifrenizi girmelisiniz.")]
        [StringLength(100, ErrorMessage = "Şifreniz en az 6 haneli olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Onay")]
        [Compare("NewPassword", ErrorMessage = "Yeni şifre ve yeni şifre onayı aynı olmalıdır.")]
        public string ConfirmPassword { get; set; }
    }
}