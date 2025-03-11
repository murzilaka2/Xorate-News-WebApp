using Microsoft.AspNetCore.Mvc;
using Xorate.Interfaces;

namespace Xorate.ViewComponents
{
    public class RelatedPostsViewComponent : ViewComponent
    {
        private readonly IPost _post;
        public RelatedPostsViewComponent(IPost post)
        {
            _post = post;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _post.GetRandomPostsAsync());
        }
    }
}
