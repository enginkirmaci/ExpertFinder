using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.AdminViewModel
{
    public class OffersViewModel
    {
        public string OfferUser { get; set; }

        public string OfferUserEmail { get; set; }

        public string SelectedCategory { get; set; }

        public string SelectedCategoryId { get; set; }

        public string SelectedSubCategory { get; set; }

        public string SelectedSubCategoryId { get; set; }

        public int PageNumber { get; set; }

        public IEnumerable<Offer> Offers { get; set; }
    }
}