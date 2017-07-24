using System.Collections.Generic;

namespace ExpertFinder.Common.ViewModels.Components
{
    public class ItemListViewModel
    {
        public int PageNumber { get; set; }

        public IEnumerable<Models.Item> Result { get; set; }
    }
}