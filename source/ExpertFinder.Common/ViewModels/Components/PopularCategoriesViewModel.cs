using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.ViewModels.Components
{
    public class PopularCategoriesViewModel
    {
        public IEnumerable<Category> PopularCategories { get; set; }
    }
}