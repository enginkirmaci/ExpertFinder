using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.AdminViewModel
{
    public class MessageInfoViewModel
    {
        public int? Province { get; set; }

        public int? District { get; set; }

        public string SelectedCategory { get; set; }

        public string SelectedCategoryId { get; set; }

        public string SelectedSubCategory { get; set; }

        public string SelectedSubCategoryId { get; set; }

        public string MessageInfo { get; set; }

        public bool IsSuccess { get; set; }

        public string Result { get; set; }

        public IEnumerable<Province> ProvinceList { get; set; }

        public IEnumerable<District> DistrictList { get; set; }
    }
}