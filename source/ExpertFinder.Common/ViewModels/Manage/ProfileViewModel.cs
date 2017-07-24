using Common.Entities.Constants;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.ViewModels.Manage
{
    public class ProfileViewModel
    {
        public string SlugUrl { get; set; }

        public string ProfileImageUrl { get; set; }

        public string AvatarUrl { get; set; }

        [Required(ErrorMessage = "Adınızı ve soyadınızı girmelisiniz.")]
        [Display(Name = "Adınızı Soyadınız")]
        public string FullName { get; set; }

        [Display(Name = "Şirket / Ünvan")]
        public string Title { get; set; }

        [Required(ErrorMessage = "E-mail adresinizi girmelisiniz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-mail adresi girmelisiniz!")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Cep telefonu numaranızı girmelisiniz.")]
        [RegularExpression(pattern: RegularExp.CELLPHONE, ErrorMessage = "Cep telefonu numaranız 11 haneli olmalıdır! Örnek: 05001234567")]
        [Display(Name = "Cep Telefonu")]
        public string CellPhone { get; set; }

        [RegularExpression(pattern: RegularExp.PHONE, ErrorMessage = "Telefon numaranız 11 haneli olmalıdır! Örnek: 02321234567")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        public IEnumerable<SelectListItem> Provinces { get; set; }

        [Required(ErrorMessage = "İl seçmelisiniz.")]
        [Display(Name = "İl")]
        public int ProvinceId { get; set; }

        public IEnumerable<SelectListItem> Districts { get; set; }

        [Display(Name = "İlçe")]
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "Adres girmelisiniz.")]
        [Display(Name = "Adres")]
        public string Address { get; set; }

        public IEnumerable<SelectListItem> UserCategories { get; set; }

        [Display(Name = "Verdiğim Hizmetler")]
        public IEnumerable<string> SelectedUserCategories { get; set; }

        [Display(Name = "Hakkımda")]
        public string Description { get; set; }

        public IEnumerable<UserExperienceImages> UserExperienceImages { get; set; }

        [Display(Name = "SMS almak istemiyorum")]
        public bool? SMSNotAllowed { get; set; }
    }
}