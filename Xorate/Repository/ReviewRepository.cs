using Microsoft.EntityFrameworkCore;
using Xorate.Data;
using Xorate.Interfaces;
using Xorate.Models;
using Xorate.Models.Pages;

namespace Xorate.Repository
{
    public class ReviewRepository : IReview
    {
        private readonly ApplicationContext _context;
        public ReviewRepository(ApplicationContext context)
        {
            _context = context;
        }

        public BasePagedList<Review> GetReviews(QueryOptions options)
        {
            return new BasePagedList<Review>(_context.Reviews.OrderByDescending(e => e.Id).Select(e=> new Review
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Ranking = e.Ranking,
                IsPublicated = e.IsPublicated,
                ShortPost = new ShortPost { Path = e.ShortPost.Path },
            }), options);
        }

        public async Task IncrementOrDecrementVotesAsync(int id, bool increment)
        {
            if (increment)
            {
                await _context.Database.ExecuteSqlRawAsync("UPDATE [Reviews] SET [Votes] += {0} WHERE [Id] = {1}", 1, id);
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync("UPDATE [Reviews] SET [Votes] += {0} WHERE [Id] = {1}", -1, id);
            }
        }

        public async Task PublishReviewAsync(int id)
        {
            await _context.Database.ExecuteSqlRawAsync("UPDATE [Reviews] SET [IsPublicated] = {0} WHERE [Id] = {1}", true, id);
        }

        public async Task RemoveReviewAsync(int id)
        {
            _context.Reviews.Remove(new Review { Id = id });
            await _context.SaveChangesAsync();
        }
    }
}
