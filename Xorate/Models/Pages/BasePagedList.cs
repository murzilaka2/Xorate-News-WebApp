using System.Linq.Expressions;

namespace Xorate.Models.Pages
{
    public class BasePagedList<T> : List<T>
    {
        public BasePagedList(IQueryable<T> query, QueryOptions? options = null)
        {
            CurrentPage = options.Page;
            PageSize = options.PageSize;
            Options = options;

            if (options != null)
            {
                if (!string.IsNullOrEmpty(options.Search))
                {
                    query = Search(query, Options.Search, Options.SearchPropertyName);
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

        private static IQueryable<T> Search(IQueryable<T> query, string searchTerm, string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var source = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
            var body = Expression.Call(source, "Contains", Type.EmptyTypes, Expression.Constant(searchTerm, typeof(string)));
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }

    }
}
