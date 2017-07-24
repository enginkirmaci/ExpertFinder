using Common.Database.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ExpertFinder.Models
{
    public class User : ApplicationUser
    {
        public User()
        {
            BoughtHistory = new HashSet<BoughtHistory>();
            Item = new HashSet<Item>();
            Notifications = new HashSet<Notifications>();
            Offer = new HashSet<Offer>();
            SpentHistory = new HashSet<SpentHistory>();
            UserCategoryRelation = new HashSet<UserCategoryRelation>();
            UserExperienceImages = new HashSet<UserExperienceImages>();
        }

        public string FullName { get; set; }

        public string Address { get; set; }

        public string AvatarUrl { get; set; }

        public string Description { get; set; }

        public int? DistrictId { get; set; }

        public int? Experience { get; set; }

        public bool? IsBoughtAnyToken { get; set; }

        public string PhoneNumber2 { get; set; }

        public string ProfileImageUrl { get; set; }

        public int? ProvinceId { get; set; }

        public double? Rating { get; set; }

        public string SlugUrl { get; set; }

        public string Title { get; set; }

        public string VerifyCode { get; set; }

        public DateTime? VerifyCodeExpireDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Website { get; set; }
        public bool? SmsNotAllowed { get; set; }

        [JsonIgnore]
        public virtual ICollection<BoughtHistory> BoughtHistory { get; set; }

        [JsonIgnore]
        public virtual ICollection<Item> Item { get; set; }

        [JsonIgnore]
        public virtual ICollection<Notifications> Notifications { get; set; }

        [JsonIgnore]
        public virtual ICollection<Offer> Offer { get; set; }

        [JsonIgnore]
        public virtual ICollection<SpentHistory> SpentHistory { get; set; }

        public virtual ICollection<UserCategoryRelation> UserCategoryRelation { get; set; }

        public virtual ICollection<UserExperienceImages> UserExperienceImages { get; set; }

        public virtual ICollection<UserRatings> UserRatings { get; set; }

        public virtual District District { get; set; }

        public virtual Province Province { get; set; }
    }
}