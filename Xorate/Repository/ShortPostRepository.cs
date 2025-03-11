using Microsoft.EntityFrameworkCore;
using Xorate.Data;
using Xorate.Helpers;
using Xorate.Interfaces;
using Xorate.Models;
using Xorate.Models.Pages;
using Xorate.ViewModels;

namespace Xorate.Repository
{
    public class ShortPostRepository : IShortPost
    {
        private readonly ApplicationContext _context;
        public ShortPostRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<int> AddShortPostsAsync(IEnumerable<ShortPost> posts)
        {
            int count = 0;
            foreach (ShortPost post in posts)
            {
                post.Path = UrlCreator.CreateUrl(post.Title);
                if (await _context.ShortPosts.AnyAsync(e => e.Path.Equals(post.Path)))
                {
                    continue;
                }
                _context.ShortPosts.Add(post);
                count += await _context.SaveChangesAsync();
            }
            return count;
        }

        public async Task<IEnumerable<string>> GetAllNamesAsync()
        {
            return await _context.ShortPosts.Select(e => e.Title).ToListAsync();
        }
        public async Task<string?> GetPreviousShortPostPathAsync(int id)
        {
            return await _context.ShortPosts.Where(e => e.Id < id).OrderByDescending(e => e.Id).Select(e => e.Path).FirstOrDefaultAsync();
        }

        public async Task<string?> GetNextShortPostPathAsync(int id)
        {
            return await _context.ShortPosts.Where(e => e.Id > id).OrderBy(e => e.Id).Select(e => e.Path).FirstOrDefaultAsync();
        }

        public async Task<ShortPost> GetShortPostAsync(int id)
        {
            return await _context.ShortPosts.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<ShortPost?> GetShortPostByPathAsync(string path)
        {
            return await _context.ShortPosts.Where(e => e.Path.Equals(path)).Select(e => new ShortPost
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                ImageLink = e.ImageLink,
                Path = e.Path,
                Ranking = e.Reviews.Any() ? e.Reviews.Average(r => r.Ranking) : 0,
                Reviews = e.Reviews.Where(r => r.IsPublicated == true).ToList()
            }).FirstOrDefaultAsync();
        }

        public BasePagedList<ShortPost> GetShortPosts(QueryOptions options)
        {
            return new BasePagedList<ShortPost>(_context.ShortPosts.Select(e => new ShortPost
            {
                Id = e.Id,
                Path = e.Path,
                Title = e.Title,
                Ranking = e.Reviews.Any() ? e.Reviews.Average(r => r.Ranking) : 0

            }).OrderByDescending(e => e.Ranking), options);
        }

        public async Task RemoveShortPostAsync(int id)
        {
            _context.ShortPosts.Remove(new ShortPost { Id = id });
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateShortPostAsync(ShortPost post)
        {
            post.Path = UrlCreator.CreateUrl(post.Title);
            if (await _context.ShortPosts.Where(e => e.Id != post.Id).AnyAsync(e => e.Path.Equals(post.Path)))
            {
                return false;
            }
            _context.ShortPosts.Update(post);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<IEnumerable<string>> GetShortPostsPathAsync()
        {
            return await _context.ShortPosts.Select(e => e.Path).ToListAsync();
        }

        public async Task<IEnumerable<ShortPost>> GetShortPostsPathTitleAsync()
        {
            return await _context.ShortPosts.OrderByDescending(e => e.Id).Select(e => new ShortPost
            {
                Path = e.Path,
                Title = e.Title
            }).ToListAsync();
        }

        public async Task AddShortPostReviewAsync(ShortPostReviewViewModel shortPostReviewViewModel)
        {
            _context.Reviews.Add(new Review
            {
                ShortPostId = shortPostReviewViewModel.PostId,
                Name = shortPostReviewViewModel.Name,
                Ranking = shortPostReviewViewModel.Ranking,
                Description = shortPostReviewViewModel.Description!
            });
            await _context.SaveChangesAsync();
        }
    }
}
