using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Xorate.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Xorate.Interfaces;
using Xorate.Models.Pages;
using Xorate.Models;
using System.Text.Json;


namespace Xorate.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [Route("/panel")]
        public IActionResult Index([FromServices] IPost posts, QueryOptions options)
        {
            return View(posts.GetPostsAdmin(options));
        }

        #region Login

        [Route("/login")]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [Route("/login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                if (Authenticate(loginViewModel))
                {
                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, loginViewModel.Email) };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    if (!string.IsNullOrEmpty(loginViewModel.ReturnUrl) && Url.IsLocalUrl(loginViewModel.ReturnUrl))
                    {
                        return Redirect(loginViewModel.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль.");
                }
            }
            return View(loginViewModel);
        }

        private bool Authenticate(LoginViewModel loginViewModel)
        {
            return loginViewModel.Email.Equals("admin@gmail.com") && loginViewModel.Password.Equals("192837");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Posts
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeletePost(int id, [FromServices] IPost posts)
        {
            await posts.RemovePostAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> HideOrShowPost(int id, [FromServices] IPost posts)
        {
            await posts.HideOrShowPostAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Route("/update")]
        public async Task<IActionResult> UpdatePost(int id, [FromServices] IPost posts)
        {
            var currentPost = await posts.GetPostByIdAsync(id);
            if (currentPost is null)
            {
                return RedirectToAction(nameof(Index));
            }
            PublicationViewModel publicationViewModel = new PublicationViewModel()
            {
                Id = currentPost.Id,
                Title = currentPost.Title,
                Description = currentPost.Description,
                Image = currentPost.Image,
                Alt = currentPost.Alt,
                SeoKeywords = currentPost.SeoKeywords,
                SeoDescription = currentPost.SeoDescription,
                Tags = currentPost.Tags,
                Rating = currentPost.Ratings?.Select(e => e.Name).ToArray()
            };
            return View(publicationViewModel);
        }

        [Route("/update")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdatePost(PublicationViewModel model, [FromServices] IPost posts)
        {
            if (ModelState.ContainsKey("CaptchaCode"))
            {
                ModelState["CaptchaCode"].Errors.Clear();
                ModelState["CaptchaCode"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }
            if (ModelState.IsValid)
            {
                Post post = new Post
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    Image = model.Image,
                    Alt = model.Alt,
                    SeoKeywords = model.SeoKeywords,
                    SeoDescription = model.SeoDescription,
                    Tags = model.Tags
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
                int result = await posts.UpdatePostAsync(post);
                if (result == 0)
                {
                    ModelState.AddModelError("Title", "Заголовок записи - не уникальный.");
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Route("/preview")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> PreviewPost(int id, [FromServices] IPost posts)
        {
            var currentPost = await posts.GetPostByIdAsync(id);
            if (currentPost is null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(new GetPostViewModel
            {
                Post = currentPost,
                CommentsCount = 0
            });
        }
        #endregion region

        #region ShortPosts

        [Route("/short-posts")]
        public IActionResult ShortPosts([FromServices] IShortPost shortPosts, QueryOptions options)
        {
            return View(shortPosts.GetShortPosts(options));
        }

        [HttpPost]
        [Route("/short-posts")]
        public async Task<IActionResult> ShortPosts([FromServices] IShortPost shortPosts, string jsonData, QueryOptions options)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    IEnumerable<ShortPost> data = JsonSerializer.Deserialize<IEnumerable<ShortPost>>(jsonData);
                    int count = await shortPosts.AddShortPostsAsync(data);
                    if (count > 0)
                    {
                        
                        TempData["AddedIds"] = JsonSerializer.Serialize(data.Select(e => e.Id).ToArray());
                        TempData["Success"] = $"Успешно добавлено {count} из {data.Count()}.";
                    }
                    else
                    {
                        TempData["Error"] = $"Добавлено {count} записей. Проверьте уникальность заголовков.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Данные не получены.";
            }
            return View(shortPosts.GetShortPosts(options));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteShortPost([FromServices] IShortPost shortPosts, int id)
        {
            await shortPosts.RemoveShortPostAsync(id);
            TempData["Success"] = $"Успешно удалено.";
            return RedirectToAction(nameof(ShortPosts));
        }

        [Route("/update-short-post")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdateShortPost([FromServices] IShortPost shortPosts, int id, string? returnUrl)
        {
            var currentPost = await shortPosts.GetShortPostAsync(id);
            if (currentPost is null)
            {
                return RedirectToAction(nameof(ShortPosts));
            }
            return View(new ShortPostViewModel
            {
                Id = currentPost.Id,
                Name = currentPost.Title,
                Description = currentPost.Description,
                ImageLink = currentPost.ImageLink,
                Ranking = currentPost.Ranking,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [Route("/update-short-post")]
        public async Task<IActionResult> UpdateShortPost([FromServices] IShortPost shortPosts, ShortPostViewModel shortPostViewModel)
        {
            if (ModelState.IsValid)
            {
                bool result = await shortPosts.UpdateShortPostAsync(new ShortPost
                {
                    Id = shortPostViewModel.Id,
                    Title = shortPostViewModel.Name,
                    Description = shortPostViewModel.Description,
                    ImageLink = shortPostViewModel.ImageLink,
                    Ranking = shortPostViewModel.Ranking
                });

                if (!result)
                {
                    ModelState.AddModelError("Name", "Такое название уже используется.");
                    return View(shortPostViewModel);
                }

                return Redirect(shortPostViewModel.ReturnUrl ?? "/short-posts");
            }
            return View(shortPostViewModel);
        }

        public async Task<IActionResult> GetAllNames([FromServices] IShortPost shortPosts)
        {
            IEnumerable<string> allNames = await shortPosts.GetAllNamesAsync();
            if (allNames != null)
            {
                return Content($"{string.Join(",", allNames)}.");
            }
            return Content("Ничего не найдено.");
        }

        #endregion

        #region Comments

        [Route("/comments")]
        public IActionResult Comments([FromServices] IComment comments, QueryOptions options)
        {
            return View(comments.GetComments(options));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteComment([FromServices] IComment comments, int id)
        {
            await comments.RemoveCommentAsync(id);
            return RedirectToAction(nameof(Comments));
        }
        #endregion

        #region Reviews

        [Route("/reviews")]
        public ActionResult Reviews([FromServices] IReview reviews, QueryOptions options)
        {
            return View(reviews.GetReviews(options));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteReview([FromServices] IReview reviews, int id)
        {
            await reviews.RemoveReviewAsync(id);
            return RedirectToAction(nameof(Reviews));
        }

        public async Task<ActionResult> PublishReview([FromServices] IReview reviews, int id)
        {
            await reviews.PublishReviewAsync(id);
            return RedirectToAction(nameof(Reviews));
        }
        #endregion
    }
}
