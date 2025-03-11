namespace Xorate.Models.Pages
{
    public class QueryOptions
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 30;
        public string? Search { get; set; }
        public string? SearchPropertyName { get; set; }
        public string? Tag { get; set; }
    }
}
