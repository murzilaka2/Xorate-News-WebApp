using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Xorate.Helpers;
using Xorate.Interfaces;
using Xorate.Models;
using Xorate.Models.Pages;
using Xorate.ViewModels;

namespace Xorate.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPost _posts;
        private readonly IMemoryCache _memoryCache;
        public HomeController(IPost posts, IMemoryCache memoryCache)
        {
            _posts = posts;
            _memoryCache = memoryCache;
        }

        public IActionResult Index(QueryOptions options)
        {
            return View(_posts.GetPosts(options));
        }

        [Route("/{path}")]
        public async Task<IActionResult> GetPost(string path, [FromServices] IComment comments)
        {
            Post currentPost;
            if (!_memoryCache.TryGetValue(path, out currentPost))
            {
                currentPost = await _posts.GetPostByPathAsync(path);
                if (currentPost != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60));
                    _memoryCache.Set(path, currentPost, cacheEntryOptions);
                }
            }

            if (!currentPost?.IsPublicated ?? true)
            {
                return NotFound();
            }

            if (currentPost != null)
            {
                if (TempData["View"] == null || TempData["View"].ToString() != currentPost.Id.ToString())
                {
                    await _posts.UpdateViewsAsync(currentPost.Id);
                    TempData["View"] = currentPost.Id.ToString();
                }

                return View(new GetPostViewModel
                {
                    Post = currentPost,
                    CommentsCount = await comments.GetPostCommentsCountAsync(currentPost.Id),
                });
            }
            else
            {
                return NotFound();
            }
        }

        [Route("/tag/{tag}")]
        public IActionResult GetPostsByTag(string tag, QueryOptions options)
        {
            return View("Index", _posts.GetPostsByTag(tag, options));
        }

        [Route("/create")]
        public IActionResult CreatePost()
        {
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        [Route("/create")]
        public async Task<IActionResult> CreatePost(PublicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, HttpContext))
                {
                    ModelState.AddModelError("CaptchaCode", "Не верный код капчи");
                    return View(model);
                }
                Post post = new Post
                {
                    Title = model.Title,
                    Description = model.Description,
                    Image = model.Image,
                    Alt = model.Alt,
                    SeoKeywords = model.SeoKeywords,
                    SeoDescription = model.SeoDescription,
                    Tags = TagsHelper.GetTags(model.Description)
                };

                if (model.Rating != null)
                {
                    model.Rating = model.Rating.Where(item => item != null).ToArray();
                    List<Rating> ratings = new List<Rating>();
                    int count = 1;
                    foreach (string item in model.Rating)
                    {
                        ratings.Add(new Rating
                        {
                            Name = item,
                            Position = count++
                        });
                    }
                    post.Ratings = ratings;
                }

                int result = await _posts.AddPostAsync(post);
                if (result == 0)
                {
                    ModelState.AddModelError("Title", "Заголовок записи - не уникальный.");
                    return View(model);
                }
                TempData["id"] = result;
                return RedirectToAction(nameof(ThankYou));
            }
            else
            {
                return View(model);
            }
        }

        [Route("/thank-you")]
        public async Task<IActionResult> ThankYou()
        {
            if (TempData["id"] is null)
            {
                return RedirectToAction(nameof(Index));
            }
            string path = await _posts.GetPostPathAsync(int.Parse(TempData["id"].ToString()));
            return View(model: (link: $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{path}", path: path));
        }

        public IActionResult GetCaptchaImage()
        {
            int width = 150;
            int height = 56;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

        public IActionResult IsImagePath(string image)
        {
            if (ImageHelper.IsImagePath(image))
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        //Google Verify
        [Route("google95a1d49143638fe1.html")]
        public IActionResult GoogleVerify()
        {
            return Content("google-site-verification: google95a1d49143638fe1.html");
        }

        public ActionResult GetSurveyFill(int id)
        {
            //Для реального голосования, получаем голос и сохраняем в базу данных.
            //Получае из базы данных голоса и передаем их в качестве модели для представления.
            TempData[id.ToString()] = id;
            return PartialView("_SurveyFill");
        }
    }
}
