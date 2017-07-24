using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ExpertFinder.Models
{
    public partial class Category
    {
        public Category()
        {
            CategoryQuestions = new HashSet<CategoryQuestions>();
            Item = new HashSet<Item>();
            UserCategoryRelation = new HashSet<UserCategoryRelation>();
        }

        public Guid Id { get; set; }

        public string Icon { get; set; }

        public string ImageUrl { get; set; }

        public bool IsUrun { get; set; }

        public string Name { get; set; }

        public Guid? ParentId { get; set; }

        public int? Priority { get; set; }

        public string SlugUrl { get; set; }

        [JsonIgnore]
        public virtual ICollection<CategoryQuestions> CategoryQuestions { get; set; }

        [JsonIgnore]
        public virtual ICollection<Item> Item { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserCategoryRelation> UserCategoryRelation { get; set; }

        public virtual Category Parent { get; set; }
        [JsonIgnore]
        public virtual ICollection<Category> ChildCategories { get; set; }
    }
}