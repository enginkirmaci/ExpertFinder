using Common.Entities.Constants;
using Common.Web.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.ViewModels.Account
{
    public class ExternalLoginConfirmationViewModel
    {
        public string ReturnUrl { get; set; }

        public string Provider { get; set; }

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

        [IsTrue(ErrorMessage = "Kullanıcı ve Gizlilik koşullarını kabul etmelisiniz.")]
        public bool PrivacyPolicy { get; set; }

        public IEnumerable<SelectListItem> UserCategories { get; set; }

        [Display(Name = "Vermek İstediğiniz Hizmetler")]
        public IEnumerable<string> SelectedUserCategories { get; set; }
    }
}