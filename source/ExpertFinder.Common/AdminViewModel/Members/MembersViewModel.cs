using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.AdminViewModel
{
    public class MembersViewModel
    {
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public int? Province { get; set; }

        public int? District { get; set; }

        public int PageNumber { get; set; }

        public IEnumerable<Province> ProvinceList { get; set; }

        public IEnumerable<District> DistrictList { get; set; }

        public IEnumerable<User> Members { get; set; }
    }
}