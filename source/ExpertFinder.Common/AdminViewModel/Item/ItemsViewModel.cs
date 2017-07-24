using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.AdminViewModel
{
    public class ItemsViewModel
    {
        public string ItemUser { get; set; }

        public string ItemUserEmail { get; set; }

        public string SelectedCategory { get; set; }

        public string SelectedCategoryId { get; set; }

        public string SelectedSubCategory { get; set; }

        public string SelectedSubCategoryId { get; set; }

        public string Status { get; set; }

        public int PageNumber { get; set; }

        public IEnumerable<Item> Items { get; set; }
    }
}