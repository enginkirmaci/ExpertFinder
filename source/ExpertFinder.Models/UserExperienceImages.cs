using Newtonsoft.Json;

namespace ExpertFinder.Models
{
    public partial class UserExperienceImages
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}