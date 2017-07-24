using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ExpertFinder.Common.ViewModels.Manage
{
    public class ProjectsViewModel
    {
        public IEnumerable<Models.Item> Items { get; set; }

        public IEnumerable<SelectListItem> FilterTypes { get; set; }

        public int FilterTypeId { get; set; }
    }
}