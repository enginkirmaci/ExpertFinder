using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.ViewModels
{
    public class CategoriesViewModel
    {
        public Category CurrentCategory { get; set; }

        public string ImageUrl { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public int PageNumber { get; set; }

        public IEnumerable<Models.Item> Items { get; set; }
    }
}