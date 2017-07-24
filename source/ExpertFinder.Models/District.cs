using Newtonsoft.Json;
using System.Collections.Generic;

namespace ExpertFinder.Models
{
    public partial class District
    {
        public District()
        {
            User = new HashSet<User>();
            Item = new HashSet<Item>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int ProvinceId { get; set; }

        [JsonIgnore]
        public virtual ICollection<User> User { get; set; }

        [JsonIgnore]
        public virtual ICollection<Item> Item { get; set; }

        [JsonIgnore]
        public virtual Province Province { get; set; }
    }
}