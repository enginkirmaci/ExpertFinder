using System;

namespace ExpertFinder.Models
{
    public partial class Content
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
    }
}