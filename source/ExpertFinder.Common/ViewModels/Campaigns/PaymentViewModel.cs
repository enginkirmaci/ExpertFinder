using ExpertFinder.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.ViewModels.Campaigns
{
    public class PaymentViewModel
    {
        public Campain Campaign { get; set; }

        public int CardType { get; set; }

        [Required(ErrorMessage = "Kart üzerindeki isminizi girmelisiniz.")]
        [Display(Name = "Kart üzerindeki İsim")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Kart numaranızı girmelisiniz.")]
        [Display(Name = "Kart Numarası")]
        public string CardNumber { get; set; }

        public IEnumerable<SelectListItem> Months { get; set; }

        [Required(ErrorMessage = "Ay seçmelisiniz.")]
        [Display(Name = "Ay")]
        public int MonthId { get; set; }

        public IEnumerable<SelectListItem> Years { get; set; }

        [Required(ErrorMessage = "Yıl seçmelisiniz.")]
        [Display(Name = "Yıl")]
        public int YearId { get; set; }

        [Required(ErrorMessage = "Güvenlik kodunu girmelisiniz.")]
        [Display(Name = "Güvenlik Kodu")]
        public string CVC { get; set; }

        [Required(ErrorMessage = "Fatura Adresi girmelisiniz.")]
        [Display(Name = "Fatura Adresi")]
        public string OrderAddress { get; set; }
    }
}