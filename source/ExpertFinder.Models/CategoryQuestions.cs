using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpertFinder.Models
{
    public partial class CategoryQuestions
    {
        public int Id { get; set; }

        public Guid? CategoryId { get; set; }

        public int CategoryQuestionTypeId { get; set; }

        public string Label { get; set; }

        public string ListValues { get; set; }

        [NotMapped]
        public string Value { get; set; }

        public int? Priority { get; set; }
        [JsonIgnore]
        public virtual Category Category { get; set; }
    }
}