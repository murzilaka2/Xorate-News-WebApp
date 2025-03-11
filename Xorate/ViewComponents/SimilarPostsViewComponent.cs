using Microsoft.AspNetCore.Mvc;
using Xorate.Interfaces;

namespace Xorate.ViewComponents
{
    public class SimilarPostsViewComponent : ViewComponent
    {
        private readonly IPost _post;
        public SimilarPostsViewComponent(IPost post)
        {
            _post = post;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            return View(await _post.GetSimilarPostsAsync(id));
        }
    }
}
