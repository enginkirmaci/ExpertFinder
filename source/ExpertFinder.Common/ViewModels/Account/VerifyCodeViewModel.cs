using ExpertFinder.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.ViewModels.Account
{
    public class VerifyCodeViewModel
    {
        public string ReaminingSeconds { get; set; }

        [Required]
        public VerificationTypes Type { get; set; }

        [Required(ErrorMessage = "Cep telefonunuza gelen onay kodunu giriniz.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Onay kodunuz 6 karakterden oluşmaktadır.")]
        [Display(Name = "Onay Kodu")]
        public string Code { get; set; }
    }
}