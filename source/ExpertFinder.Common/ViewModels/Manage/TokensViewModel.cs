using System;
using System.Collections.Generic;

namespace ExpertFinder.Common.ViewModels.Manage
{
    public class TokensViewModel
    {
        public IEnumerable<TokenItem> Items { get; set; }

        public int TokenCount { get; set; }
    }

    public class TokenItem
    {
        public string Title { get; set; }

        public DateTime? Date { get; set; }

        public int? Count { get; set; }
    }
}