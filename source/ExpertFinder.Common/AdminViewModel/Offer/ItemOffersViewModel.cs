using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.AdminViewModel
{
    public class ItemOffersViewModel
    {
        public IEnumerable<Offer> Offers { get; set; }

        public Item Item { get; set; }
        public IEnumerable<CategoryQuestions> Questions { get; set; }
        public string UIUrl { get; set; }
    }
}