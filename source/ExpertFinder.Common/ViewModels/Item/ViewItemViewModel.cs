using ExpertFinder.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.ViewModels.Item
{
    public class ViewItemViewModel
    {
        public User ItemUser { get; set; }

        public Models.Item Item { get; set; }

        public bool hasWinner { get; set; }

        public IEnumerable<Offer> Offers { get; set; }

        public IEnumerable<CategoryQuestions> Questions { get; set; }

        public IEnumerable<string> Images { get; set; }

        [Required(ErrorMessage = "Mesaj girmelisiniz.")]
        [Display(Name = "Mesajınız")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Fiyat girmelisiniz.")]
        [RegularExpression(@"^\d*(\,|(,\d{1,2}))?$", ErrorMessage = "Geçerli bir fiyat girmelisiniz.")]

        //[MaxLength(10, ErrorMessage = "Toplam 10 haneden fazla sayı giremezsiniz.")]
        [Display(Name = "Fiyat")]
        public decimal? OfferPrice { get; set; }
    }
}