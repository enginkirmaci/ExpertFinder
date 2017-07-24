using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.AdminViewModel
{
    public class CampaignFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kampanya adını giriniz.")]
        [Display(Name = "Kampanya Adı")]
        public string Title { get; set; }

        [Display(Name = "Kampanya Açıklama")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Jeton adetini giriniz.")]
        [Display(Name = "Jeton Adeti")]
        public int? TokenCount { get; set; }

        [Required(ErrorMessage = "Kampanya ücretini giriniz.")]
        [Display(Name = "Kampanya Ücreti")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Kampanya önceliğini giriniz.")]
        [Display(Name = "Kampanya Önceliği")]
        public int? Priority { get; set; }

        [Required(ErrorMessage = "Kampanya aktiflik durumunu seçiniz.")]
        [Display(Name = "Aktif")]
        public string IsActive { get; set; }

        [Required(ErrorMessage = "Kampanya ücret durumunu seçiniz.")]
        [Display(Name = "Ücretsiz")]
        public string IsFree { get; set; }

        public string SlugUrl { get; set; }

        public string Url { get; set; }

        public string IsFreeForDisabledWidget { get; set; }

        public string UIUrl { get; set; }
    }
}