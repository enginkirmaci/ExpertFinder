using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Common.AdminViewModel
{
    public class CampaignViewModel
    {
        public IEnumerable<Campain> ActiveCampaign { get; set; }

        public IEnumerable<Campain> PassiveCampaign { get; set; }
    }
}