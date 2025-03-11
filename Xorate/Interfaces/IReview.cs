using Xorate.Models.Pages;
using Xorate.Models;

namespace Xorate.Interfaces
{
    public interface IReview
    {
        BasePagedList<Review> GetReviews(QueryOptions options);
        Task PublishReviewAsync(int id);
        Task RemoveReviewAsync(int id);
        Task IncrementOrDecrementVotesAsync(int id, bool increment);
    }
}
