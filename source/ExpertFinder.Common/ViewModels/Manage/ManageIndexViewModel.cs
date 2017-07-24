using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.ViewModels.Manage
{
    public class ManageIndexViewModel
    {
        public IEnumerable<Notifications> Notifications { get; set; }

        public IEnumerable<UserCategoryRelation> UserCategories { get; set; }
    }
}