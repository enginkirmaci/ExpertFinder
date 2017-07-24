using System;
using System.Collections.Generic;

namespace ExpertFinder.Models
{
    public partial class Offer
    {
        public Offer()
        {
            UserRatings = new HashSet<UserRatings>();
        }

        public Guid Id { get; set; }

        public string Comment { get; set; }

        public bool IsRated { get; set; }

        public bool? IsWinner { get; set; }

        public Guid? ItemId { get; set; }

        public DateTime? OfferDate { get; set; }

        public decimal? OfferPrice { get; set; }

        public int? TokenSpent { get; set; }

        public string UserId { get; set; }

        public virtual ICollection<UserRatings> UserRatings { get; set; }

        public virtual Item Item { get; set; }

        public virtual User User { get; set; }
    }
}