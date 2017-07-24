using System.Collections.Generic;

namespace ExpertFinder.Models
{
    public partial class Campain
    {
        public Campain()
        {
            BoughtHistory = new HashSet<BoughtHistory>();
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsFree { get; set; }
        public decimal? Price { get; set; }
        public int? Priority { get; set; }
        public string SlugUrl { get; set; }
        public string Title { get; set; }
        public int? TokenCount { get; set; }
        public string Url { get; set; }

        public virtual ICollection<BoughtHistory> BoughtHistory { get; set; }
    }
}