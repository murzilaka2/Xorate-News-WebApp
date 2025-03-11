using Xorate.Models;
using Xorate.Models.Pages;
using Xorate.ViewModels;

namespace Xorate.Interfaces
{
    public interface IShortPost
    {
        Task<int> AddShortPostsAsync(IEnumerable<ShortPost> posts);
        Task<bool> UpdateShortPostAsync(ShortPost post);
        Task RemoveShortPostAsync(int id);

        Task<ShortPost> GetShortPostAsync(int id);
        Task<ShortPost?> GetShortPostByPathAsync(string path);
        BasePagedList<ShortPost> GetShortPosts(QueryOptions options);
        Task<IEnumerable<string>> GetAllNamesAsync();

        Task<string?> GetPreviousShortPostPathAsync(int id); 
        Task<string?> GetNextShortPostPathAsync(int id);
        Task<IEnumerable<string>> GetShortPostsPathAsync();
        Task<IEnumerable<ShortPost>> GetShortPostsPathTitleAsync();

        Task AddShortPostReviewAsync(ShortPostReviewViewModel shortPostReviewViewModel);
    }
}
