using Microsoft.EntityFrameworkCore;
using Xorate.Data;
using Xorate.Helpers;
using Xorate.Interfaces;
using Xorate.Models;
using Xorate.Models.Pages;

namespace Xorate.Repository
{
    public class PostRepository : IPost
    {
        private readonly ApplicationContext _context;

        public PostRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<int> AddPostAsync(Post post)
        {
            post.Path = UrlCreator.CreateUrl(post.Title);

            if (await _context.Posts.AnyAsync(e => e.Path.Equals(post.Path)))
            {
                return 0;
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post.Id;
        }
        public async Task<int> UpdatePostAsync(Post post)
        {
            post.Path = UrlCreator.CreateUrl(post.Title);
            if (await _context.Posts.Where(e => e.Id != post.Id).AnyAsync(e => e.Path.Equals(post.Path)))
            {
                return 0;
            }
            var currentPost = await _context.Posts.Include(e => e.Ratings).FirstOrDefaultAsync(e => e.Id == post.Id);
            if (currentPost != null)
            {
                _context.Entry(currentPost).State = EntityState.Modified;
                _context.Entry(currentPost).Property(e => e.CreatedDate).IsModified = false;
                _context.Entry(currentPost).Property(e => e.IsPublicated).IsModified = false;
                _context.Entry(currentPost).Property(e => e.Views).IsModified = false;

                currentPost.Title = post.Title;
                currentPost.Description = post.Description;
                currentPost.Alt = post.Alt;
                currentPost.SeoDescription = post.SeoDescription;
                currentPost.SeoKeywords = post.SeoKeywords;
                currentPost.Tags = post.Tags;
                currentPost.Image = post.Image;
                currentPost.Ratings = post.Ratings;
                await _context.SaveChangesAsync();
            }
            return post.Id;
        }
        public async Task<Post?> GetPostByIdAsync(int id)
        {
            return await _context.Posts.Include(e => e.Ratings).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Post?> GetPostByPathAsync(string path)
        {
            return await _context.Posts.Where(e => e.IsPublicated == true).Include(e => e.Ratings).FirstOrDefaultAsync(e => e.Path.Equals(path));
        }

        public async Task<string?> GetPostPathAsync(int postId)
        {
            return await _context.Posts.Where(e => e.Id == postId)
                .Select(e => e.Path).FirstOrDefaultAsync();
        }

        public PagedList<Post> GetPosts(QueryOptions options)
        {
            return new PagedList<Post>(_context.Posts.Where(e => e.IsPublicated == true).OrderByDescending(e => e.Id).Select(e => new Post
            {
                Id = e.Id,
                Path = e.Path,
                Title = e.Title,
                CreatedDate = e.CreatedDate,
                SeoDescription = e.SeoDescription,
                SeoKeywords = e.SeoKeywords,
                Likes = e.Likes,
                Views = e.Views,
                Comments = e.Comments.Select(c => new Comment { Id = c.Id }).ToList()
            }), options);
        }

        public PagedList<Post> GetPostsAdmin(QueryOptions options)
        {
            return new PagedList<Post>(_context.Posts.OrderByDescending(e => e.Id), options);
        }

        public async Task<IEnumerable<Post>> GetSimilarPostsAsync(int id)
        {
            return await _context.Posts.Where(e => e.IsPublicated == true && e.Id != id).OrderBy(e => Guid.NewGuid()).Take(3).ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetRandomPostsAsync()
        {
            return await _context.Posts.Where(e => e.IsPublicated == true).OrderBy(e => Guid.NewGuid()).Take(3).ToListAsync();
        }

        public async Task HideOrShowPostAsync(int id)
        {
            var result = await _context.Posts.Where(e => e.Id == id).Select(e => e.IsPublicated).FirstOrDefaultAsync();
            if (result)
            {
                await _context.Database.ExecuteSqlRawAsync("UPDATE [Posts] SET [IsPublicated] = {0} WHERE [Id] = {1}", false, id);
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync("UPDATE [Posts] SET [IsPublicated] = {0} WHERE [Id] = {1}", true, id);
            }
        }

        public async Task RemovePostAsync(int id)
        {
            _context.Posts.Remove(new Post { Id = id });
            await _context.SaveChangesAsync();
        }

        public async Task UpdateViewsAsync(int id)
        {
            await _context.Database.ExecuteSqlRawAsync("UPDATE [Posts] SET [Views] += 1 WHERE [Id] = {0}", id);
        }

        public async Task<IEnumerable<string>> GetPostsPathAsync()
        {
            return await _context.Posts.Where(e => e.IsPublicated).Select(e => e.Path).ToListAsync();
        }

        public PagedList<Post> GetPostsByTag(string tag, QueryOptions options)
        {
            return new PagedList<Post>(_context.Posts.Where(e => e.IsPublicated == true && e.Tags.Contains(tag)).OrderByDescending(e => e.Id).Select(e => new Post
            {
                Id = e.Id,
                Path = e.Path,
                Title = e.Title,
                CreatedDate = e.CreatedDate,
                SeoDescription = e.SeoDescription,
                SeoKeywords = e.SeoKeywords,
                Likes = e.Likes,
                Views = e.Views,
                Comments = e.Comments.Select(c => new Comment { Id = c.Id }).ToList()
            }), options);
        }

        public async Task<IEnumerable<Post>> GetPostsPathTitleViewsAsync()
        {
            return await _context.Posts.Where(e => e.IsPublicated).OrderByDescending(e => e.Id).Select(e => new Post
            {
                Path = e.Path,
                Title = e.Title,
                Views = e.Views
            }).ToListAsync();
        }
    }
}
