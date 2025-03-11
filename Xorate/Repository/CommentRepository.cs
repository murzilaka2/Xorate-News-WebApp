using Microsoft.EntityFrameworkCore;
using Xorate.Data;
using Xorate.Interfaces;
using Xorate.Models;
using Xorate.Models.Pages;

namespace Xorate.Repository
{
    public class CommentRepository : IComment
    {
        private readonly ApplicationContext _context;
        public CommentRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<int> GetPostCommentsCountAsync(int id)
        {
            return await _context.Comments.Where(e => e.PostId == id).CountAsync();
        }

        public BasePagedList<Comment> GetComments(QueryOptions options)
        {
            return new BasePagedList<Comment>(_context.Comments.OrderByDescending(e => e.Id).Select(e => new Comment
            {
                Id = e.Id,
                CreatedDate = e.CreatedDate,
                Email = e.Email,
                Name = e.Name,
                Text = e.Text,
                Post = new Post
                {
                    Path = e.Post.Path
                }
            }), options);
        }

        public async Task RemoveCommentAsync(int id)
        {
            _context.Comments.Remove(new Comment { Id = id });
            await _context.SaveChangesAsync();
        }
    }
}
