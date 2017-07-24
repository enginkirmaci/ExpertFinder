using ExpertFinder.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ExpertFinder.Common.ViewModels.Manage
{
    public class OffersViewModel
    {
        public IEnumerable<Offer> Offers { get; set; }

        public IEnumerable<SelectListItem> FilterTypes { get; set; }

        public int FilterTypeId { get; set; }
    }
}