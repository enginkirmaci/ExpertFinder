using Newtonsoft.Json;
using System;

namespace ExpertFinder.Models
{
    public partial class UserRatings
    {
        public Guid Id { get; set; }

        public string Comment { get; set; }

        public string UserId { get; set; }

        public Guid? ItemId { get; set; }

        public int RateValue { get; set; }

        public Guid? WinnerOfferId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        public virtual Item Item { get; set; }

        public virtual Offer WinnerOffer { get; set; }
    }
}