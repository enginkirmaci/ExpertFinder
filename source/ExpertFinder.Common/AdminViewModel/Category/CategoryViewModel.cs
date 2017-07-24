using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.AdminViewModel
{
    public class CategoryViewModel
    {
        public IEnumerable<Category> Categories { get; set; }

        public Category Category { get; set; }
    }
}