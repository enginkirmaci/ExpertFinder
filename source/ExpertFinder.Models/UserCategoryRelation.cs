using Newtonsoft.Json;
using System;

namespace ExpertFinder.Models
{
    public partial class UserCategoryRelation
    {
        public Guid Id { get; set; }

        public Guid? CategoryId { get; set; }

        public int? RecievedOffersCount { get; set; }

        public DateTime? SelectDate { get; set; }

        public string UserId { get; set; }

        public virtual Category Category { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}