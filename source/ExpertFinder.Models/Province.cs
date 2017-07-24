using Newtonsoft.Json;
using System.Collections.Generic;

namespace ExpertFinder.Models
{
    public partial class Province
    {
        public Province()
        {
            User = new HashSet<User>();
            District = new HashSet<District>();
            Item = new HashSet<Item>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int? Priority { get; set; }

        [JsonIgnore]
        public virtual ICollection<User> User { get; set; }

        [JsonIgnore]
        public virtual ICollection<District> District { get; set; }

        [JsonIgnore]
        public virtual ICollection<Item> Item { get; set; }
    }
}