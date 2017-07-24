using System;

namespace ExpertFinder.Models
{
    public partial class SpentHistory
    {
        public Guid Id { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public int? TokenSpent { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}