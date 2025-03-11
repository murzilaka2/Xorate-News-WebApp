using System.Linq.Expressions;

namespace Xorate.Models.Pages
{
    public class PagedList<T> : List<T> where T : Post
    {
        public PagedList(IQueryable<T> query, QueryOptions? options = null)
        {
            if (options.Page == 0)
            {
                options.Page = 1;
            }

            CurrentPage = options.Page;
            PageSize = options.PageSize;
            Options = options;

            if (options != null)
            {
                if (!string.IsNullOrEmpty(options.Search))
                {
                    query = Search(query, options.Search);
                }
            }
            TotalPages = (int)Math.Ceiling((decimal)query.Count() / PageSize);
            AddRange(query.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
        }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public QueryOptions Options { get; set; }

        private static IQueryable<T> Search(IQueryable<T> query, string searchTerm)
        {
            return query.Where(e => e.Title.Contains(searchTerm) || e.SeoDescription.Contains(searchTerm) || e.SeoKeywords.Contains(searchTerm));
        }
    }
}
