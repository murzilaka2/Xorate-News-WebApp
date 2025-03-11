using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using Xorate.Interfaces;
using Xorate.Models.Pages;
using Xorate.ViewModels;

namespace Xorate.Controllers
{
    public class ShortPostController : Controller
    {
        private readonly IShortPost _shortPost;
        public ShortPostController(IShortPost shortPost)
        {
            _shortPost = shortPost;
        }

        [Route("/ratings")]
        public IActionResult Index(QueryOptions options)
        {
            options.PageSize = 100;
            return View(_shortPost.GetShortPosts(options));
        }

        [Route("/ratings/{path}")]
        public async Task<IActionResult> GetItem(string path)
        {
            var item = await _shortPost.GetShortPostByPathAsync(path);
            if (item == null)
            {
                return RedirectToAction(nameof(Index));
            }

            item.ShortPostReviewViewModel = new ShortPostReviewViewModel { PostId = item.Id };
            ViewBag.PreviousShortPostPath = await _shortPost.GetPreviousShortPostPathAsync(item.Id);
            ViewBag.NextShortPostPath = await _shortPost.GetNextShortPostPathAsync(item.Id);
            return View(item);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ShortPostReview(ShortPostReviewViewModel shortPostReviewViewModel)
        {
            if (ModelState.IsValid)
            {
                await _shortPost.AddShortPostReviewAsync(shortPostReviewViewModel);
                TempData["ShortPostReview"] = shortPostReviewViewModel.PostId;
                return Content("""
                    <div class="alert alert-success" role="alert"> Ваш отзыв, успешно отправлен на модерацию! </div>
                    """);
            }
            return Content("""
                <div class="alert alert-danger" role="alert"> Произошла ошибка! Попробуйте позже. </div>
                """);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> GetVote([FromServices] IReview reviews, int reviewId, bool increment, int votes, int postId)
        {
            if (TempData[postId.ToString()] is null)
            {
                TempData[postId.ToString()] = $"|{reviewId}|";
            }
            else if (!TempData[postId.ToString()].ToString().Contains($"|{reviewId}|"))
            {
                TempData[postId.ToString()] += $"|{reviewId}|";
            }
            await reviews.IncrementOrDecrementVotesAsync(reviewId, increment);
            votes = increment ? votes + 1 : votes - 1;
            return Content($"""
                <i class="bi bi-arrow-down-square-fill"></i>
                <i class="bi bi-arrow-up-square-fill"></i>
                ({votes})
                """);
        }
    }
}
