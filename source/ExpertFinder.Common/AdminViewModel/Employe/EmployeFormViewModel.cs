using Common.Entities.Constants;
using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.AdminViewModel
{
    public class EmployeFormViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Adınızı ve soyadınızı girmelisiniz.")]
        [Display(Name = "Adınızı Soyadınız")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "E-mail adresinizi girmelisiniz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-mail adresi girmelisiniz!")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Cep telefonu numaranızı girmelisiniz.")]
        [RegularExpression(pattern: RegularExp.CELLPHONE, ErrorMessage = "Cep telefonu numaranız 11 haneli olmalıdır! Örnek: 05001234567")]
        [Display(Name = "Cep Telefonu")]
        public string CellPhone { get; set; }

        [Display(Name = "Durumu")]
        [Required(ErrorMessage = "Kullanıcı durumunu seçmelisiniz.")]
        public string IsActive { get; set; }
    }
}