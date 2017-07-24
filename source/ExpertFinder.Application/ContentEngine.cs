using Common.Web.Data;
using ExpertFinder.Application.Interfaces;
using ExpertFinder.Models;
using System.Collections.Generic;
using System.Linq;

namespace ExpertFinder.Application
{
    public class ContentEngine : IContentEngine
    {
        private readonly teklifcepteDBContext _dbContext;
        private readonly ICachingEx _cachingEx;

        public Dictionary<string, Content> Contents
        {
            get
            {
                if (!_cachingEx.Exists("Contents"))
                {
                    var contents = _dbContext.Content.ToList();
                    var contentsDictionary = new Dictionary<string, Content>();

                    foreach (var item in contents)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Value))
                            item.Value = item.Value.Trim();

                        contentsDictionary.Add(item.Key, item);
                    }
                    _cachingEx.Add("Contents", contentsDictionary);
                }

                return _cachingEx.Get<Dictionary<string, Content>>("Contents");
            }
        }

        public ContentEngine(
            teklifcepteDBContext dbContext,
            ICachingEx cachingEx)
        {
            _dbContext = dbContext;
            _cachingEx = cachingEx;
        }
    }
}