using ExpertFinder.Models;
using System.Collections.Generic;

namespace ExpertFinder.Application.Interfaces
{
    public interface IContentEngine
    {
        Dictionary<string, Content> Contents { get; }
    }
}