using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ExpertFinder.Common.ViewModels.Item
{
    public class SearchViewModel
    {
        public int PageNumber { get; set; }

        public IEnumerable<Models.Item> Result { get; set; }

        public string Keyword { get; set; }

        public IEnumerable<SelectListItem> SearchSortTypes { get; set; }

        public int SearchSortTypeId { get; set; }

        public IEnumerable<SelectListItem> Provinces { get; set; }

        public int? ProvinceId { get; set; }

        public IEnumerable<SelectListItem> Districts { get; set; }

        public int? DistrictId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public IEnumerable<string> SelectedCategories { get; set; }
    }
}