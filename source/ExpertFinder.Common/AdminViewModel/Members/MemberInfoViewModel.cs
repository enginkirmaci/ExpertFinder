using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.AdminViewModel
{
    public class MemberInfoViewModel
    {
        public User Member { get; set; }

        public IEnumerable<Offer> MemberOffers { get; set; }

        public List<Category> MemberCategoryRelationList { get; set; }

        public IEnumerable<Item> MemberItems { get; set; }

        public int TokenCount { get; set; }
    }
}