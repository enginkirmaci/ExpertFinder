using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.AdminViewModel
{
    public class CategoryQuestionsViewModel
    {
        public Category Category { get; set; }

        public IEnumerable<CategoryQuestions> CategoryQuestions { get; set; }

        public int QuestionCount { get; set; }
    }
}