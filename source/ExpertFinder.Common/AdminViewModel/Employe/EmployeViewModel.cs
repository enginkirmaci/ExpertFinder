using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.AdminViewModel
{
    public class EmployeViewModel
    {
        public IEnumerable<User> AdminUsers { get; set; }
    }
}