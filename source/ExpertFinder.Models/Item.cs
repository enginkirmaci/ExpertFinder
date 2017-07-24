using System;
using System.Collections.Generic;

namespace ExpertFinder.Models
{
    public partial class Item
    {
        public Item()
        {
            Offer = new HashSet<Offer>();
            UserRatings = new HashSet<UserRatings>();
        }

        public Guid Id { get; set; }

        public DateTime? AddedDate { get; set; }

        public Guid? CategoryId { get; set; }

        public string Description { get; set; }

        public int? DistrictId { get; set; }

        public string ItemCategoryQuestionsJSON { get; set; }

        public decimal? Price { get; set; }

        public int? ProvinceId { get; set; }

        public int? SeenCount { get; set; }

        public string SlugUrl { get; set; }

        public int? StatusID { get; set; }

        public string Title { get; set; }

        public int? TokenSpent { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string UserId { get; set; }

        public int WhenType { get; set; }

        public DateTime WhenDate { get; set; }

        public bool IsRated { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrl2 { get; set; }
        public string ImageUrl3 { get; set; }
        public string ImageUrl4 { get; set; }
        public string ImageUrl5 { get; set; }

        public virtual ICollection<Offer> Offer { get; set; }

        public virtual ICollection<UserRatings> UserRatings { get; set; }

        public virtual Category Category { get; set; }

        public virtual District District { get; set; }

        public virtual Province Province { get; set; }

        public virtual User User { get; set; }
    }
}