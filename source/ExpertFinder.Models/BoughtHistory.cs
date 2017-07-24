using System;

namespace ExpertFinder.Models
{
    public partial class BoughtHistory
    {
        public Guid Id { get; set; }

        public int? CampainId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public bool IsPaid { get; set; }

        public string OrderAddress { get; set; }

        public string UserId { get; set; }

        public virtual Campain Campain { get; set; }

        public virtual User User { get; set; }
    }
}