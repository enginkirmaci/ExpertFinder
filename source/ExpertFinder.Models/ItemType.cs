using System.Collections.Generic;

namespace ExpertFinder.Models
{
    public partial class ItemType
    {
        public ItemType()
        {
            Item = new HashSet<Item>();
        }

        public int ID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Item> Item { get; set; }
    }
}