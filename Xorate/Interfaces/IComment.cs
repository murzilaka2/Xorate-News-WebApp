using Xorate.Models;
using Xorate.Models.Pages;

namespace Xorate.Interfaces
{
    public interface IComment
    {
        Task<int> GetPostCommentsCountAsync(int id);
        BasePagedList<Comment> GetComments(QueryOptions options);
        Task RemoveCommentAsync(int id);
    }
}
