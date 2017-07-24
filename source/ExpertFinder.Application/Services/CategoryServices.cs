using Common.Web.Data;
using ExpertFinder.Domain.Interfaces;
using ExpertFinder.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpertFinder.Domain
{
    public class CategoryServices : ICategoryServices
    {
        private readonly teklifcepteDBContext _dbContext;
        private readonly ICachingEx _cachingEx;

        public CategoryServices(
            teklifcepteDBContext dbContext,
            ICachingEx cachingEx
            )
        {
            _dbContext = dbContext;
            _cachingEx = cachingEx;
        }

        public List<Category> GetCategories()
        {
            if (!_cachingEx.Exists("Categories"))
            {
                var categories = _dbContext.Category
                    .Include(i => i.ChildCategories)
                    .OrderBy(i => i.Name)
                    .ToList();

                //foreach (var category in categories)
                //{
                //    category.SlugUrl = UrlConverter.ToUrlSlug(category.Name);

                //    foreach (var item in category.ChildCategories)
                //        item.SlugUrl = UrlConverter.ToUrlSlug(item.Name);
                //}
                //_dbContext.SaveChanges();

                _cachingEx.Add("Categories", categories);
            }

            return _cachingEx.Get<List<Category>>("Categories");
        }

        public List<Category> GetPopularCategories()
        {
            if (!_cachingEx.Exists("PopularCategories"))
            {
                var categories = GetCategories()
                    .Where(i => i.Priority.HasValue) // && i.Icon != null)
                    .OrderBy(i => i.Priority)
                    .Take(12)
                    .ToList();

                _cachingEx.Add("PopularCategories", categories);
            }

            return _cachingEx.Get<List<Category>>("PopularCategories");
        }

        public Category GetCategory(string slugUrl)
        {
            var category = GetCategories()
                .FirstOrDefault(i => i.SlugUrl == slugUrl);

            return category;
        }

        public Category GetCategoryById(Guid ID)
        {
            var category = GetCategories()
                .FirstOrDefault(i => i.Id == ID);

            return category;
        }
    }
}