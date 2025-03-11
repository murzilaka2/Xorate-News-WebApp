using Xorate.Models;
using Xorate.Models.Pages;

namespace Xorate.Interfaces
{
    public interface IPost
    {
        Task<string?> GetPostPathAsync(int postId);
        Task<int> AddPostAsync(Post post);
        Task<int> UpdatePostAsync(Post post);
        Task<Post?> GetPostByPathAsync(string path);
        Task<Post?> GetPostByIdAsync(int id);
        PagedList<Post> GetPosts(QueryOptions options);
        PagedList<Post> GetPostsByTag(string tag, QueryOptions options);
        PagedList<Post> GetPostsAdmin(QueryOptions options);
        Task<IEnumerable<Post>> GetSimilarPostsAsync(int id);
        Task<IEnumerable<Post>> GetRandomPostsAsync();
        Task<IEnumerable<string>> GetPostsPathAsync();
        Task<IEnumerable<Post>> GetPostsPathTitleViewsAsync();

        Task RemovePostAsync(int id);
        Task HideOrShowPostAsync(int id);
        Task UpdateViewsAsync(int id);
    }
}
