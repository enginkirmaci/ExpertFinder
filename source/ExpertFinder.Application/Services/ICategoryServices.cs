using ExpertFinder.Models;
using System;
using System.Collections.Generic;

namespace ExpertFinder.Domain.Interfaces
{
    public interface ICategoryServices
    {
        List<Category> GetCategories();

        List<Category> GetPopularCategories();

        Category GetCategory(string slugUrl);

        Category GetCategoryById(Guid ID);
    }
}