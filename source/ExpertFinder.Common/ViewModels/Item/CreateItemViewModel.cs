using ExpertFinder.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.ViewModels.Item
{
    public class CreateItemViewModel
    {
        public bool HasCategorySelected { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        [Required(ErrorMessage = "İstediğiniz hizmet için kategori seçmelisiniz.")]
        public string SelectedCategory { get; set; }

        public IEnumerable<CategoryQuestions> Questions { get; set; }

        [Display(Name = "Talep başlığı")]
        [Required(ErrorMessage = "İstediğiniz hizmet için başlık girmelisiniz.")]
        public string Title { get; set; }

        [Display(Name = "İhtiyacın detayları neler?")]
        [Required(ErrorMessage = "İstediğiniz hizmet için açıklama giriniz.")]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> Provinces { get; set; }

        [Required(ErrorMessage = "İl seçmelisiniz.")]
        [Display(Name = "İl")]
        public int ProvinceId { get; set; }

        public IEnumerable<SelectListItem> Districts { get; set; }

        [Display(Name = "İlçe")]
        public int DistrictId { get; set; }

        public IEnumerable<SelectListItem> WhenTypes { get; set; }

        [Required(ErrorMessage = "Belirli bir zaman seçmelisiniz.")]
        public int WhenTypeId { get; set; }

        public IEnumerable<SelectListItem> WhenDates { get; set; }

        public string WhenDateId { get; set; }

        public IEnumerable<SelectListItem> WhenTimes { get; set; }

        public string WhenTimeId { get; set; }

        public decimal? Price { get; set; }

        [Display(Name = "Bütçe konusunda bir fikrim yok.")]
        public bool UnknownPrice { get; set; }
    }
}