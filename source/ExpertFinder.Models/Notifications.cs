using Newtonsoft.Json;
using System;

namespace ExpertFinder.Models
{
    public partial class Notifications
    {
        public Guid Id { get; set; }

        public DateTime? AddedDate { get; set; }

        public string Description { get; set; }

        public bool? IsSeen { get; set; }

        public Guid? ItemID { get; set; }

        public string ItemUserId { get; set; }

        public int? NotificationTypeId { get; set; }

        public DateTime? SeenDate { get; set; }

        public string Title { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}