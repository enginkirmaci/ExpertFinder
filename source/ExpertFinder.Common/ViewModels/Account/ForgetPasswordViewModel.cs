using Common.Entities.Constants;
using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.ViewModels.Account
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "Cep telefonu numaranızı girmelisiniz.")]
        [RegularExpression(pattern: RegularExp.CELLPHONE, ErrorMessage = "Cep telefonu numaranız 11 haneli olmalıdır! Örnek: 05001234567")]
        [Display(Name = "Cep Telefonu")]
        public string CellPhone { get; set; }
    }
}