using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.ViewModels
{
    public class WhereWeViewModel
    {
        public Province CurrentProvince { get; set; }

        public District CurrentDistrict { get; set; }

        public IEnumerable<Province> Provinces { get; set; }

        public IEnumerable<District> Districts { get; set; }

        public int PageNumber { get; set; }

        public IEnumerable<Models.Item> Items { get; set; }
    }
}