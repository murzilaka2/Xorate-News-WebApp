using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xorate.Helpers;
using Xorate.Interfaces;
using Xorate.Models;
using Xorate.Models.Transfers;

namespace Xorate.Controllers
{
    public class ServiceController : Controller
    {
        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [Route("/error")]
        public IActionResult Error()
        {
            return View();
        }

        [Route("/sitemap.xml")]
        public async Task<ActionResult> SitemapXml([FromServices] MapGenerator mapGenerator)
        {
            string host = Request.Scheme + "://" + Request.Host;
            var sitemapNodes = await mapGenerator.GetSitemapNodesAsync(this.Url);
            string xml = mapGenerator.GetSitemapDocument(sitemapNodes);
            return Content(xml, "text/xml", Encoding.UTF8);
        }

        [Route("/shortposts-sitemap.xml")]
        public async Task<ActionResult> ShortPostsSitemapXml([FromServices] MapGenerator mapGenerator)
        {
            string host = Request.Scheme + "://" + Request.Host;
            var sitemapNodes = await mapGenerator.GetSitemapShortPostsAsync(this.Url);
            string xml = mapGenerator.GetSitemapDocument(sitemapNodes);
            return Content(xml, "text/xml", Encoding.UTF8);
        }

        [Route("/sitemap")]
        public async Task<IActionResult> SitemapHtml([FromServices] IPost post)
        {
            return View(await post.GetPostsPathTitleViewsAsync());
        }

        [Route("/shortposts-sitemap")]
        public async Task<IActionResult> ShortPostsSitemapHtml([FromServices] IShortPost post)
        {
            return View(await post.GetShortPostsPathTitleAsync());
        }

        [Route("/robots.txt")]
        public async Task<ActionResult> RobotsTxt([FromServices] IWebHostEnvironment environment)
        {
            var robotsTxtPath = Path.Combine(environment.ContentRootPath, "robots.txt");
            string output = "User-agent: *  \nDisallow: /login\nDisallow: /?search=\nDisallow: /?page=\nDisallow: /tag/\nDisallow: /Home/GetSurveyFill\nDisallow: /Home/GetCaptchaImage\n\nSitemap: https://xorate.com/sitemap.xml";
            if (System.IO.File.Exists(robotsTxtPath))
            {
                output = await System.IO.File.ReadAllTextAsync(robotsTxtPath);
            }
            return Content(output, "text/plain", Encoding.UTF8);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult ClearCache([FromServices] IMemoryCache memoryCache)
        {
            if (memoryCache is MemoryCache cache)
            {
                cache.Clear();
            }
            return Content("Cache cleared.");
        }

        [Route("/broken-images")]
        public async Task<IActionResult> BrokenImages([FromServices] IService service)
        {
            int count = 0;
            StringBuilder sb = new StringBuilder("Битые изображения постов:<br /><br />");
            IEnumerable<BrokenImage> postsImages = await service.GetPostImagesAsync();
            foreach (BrokenImage item in postsImages)
            {
                if (!await CheckImage(item.ImageLink))
                {
                    count++;
                    string pageUrl = Url.Action(action: "GetPost", controller: "Home", values: new { path = item.Path }, protocol: "https");
                    sb.Append($"<a href=\"{pageUrl}\" target=\"_blank\">{item.Path}</a> | <a href=\"/update?id={item.Id}\" target=\"_blank\">Редактировать</a> | {item.ImageLink}<br />");
                }
            }

            sb.Append("<br /><br /><br />Битые изображения коротких постов:<br /><br />");
            IEnumerable<BrokenImage> shortPostsImages = await service.GetShortPostImagesAsync();
            foreach (BrokenImage item in shortPostsImages)
            {
                if (!await CheckImage(item.ImageLink))
                {
                    count++;
                    string pageUrl = Url.Action(action: "GetItem", controller: "ShortPost", values: new { path = item.Path }, protocol: "https");
                    sb.Append($"<a href=\"{pageUrl}\" target=\"_blank\">{item.Path}</a> | <a href=\"/update-short-post?id={item.Id}\" target=\"_blank\">Редактировать</a> | {item.ImageLink}<br />");
                }
            }
            Response.ContentType = "text/html;charset=utf-8;";
            if (count == 0)
            {
                return Content("Битых изображений не найдено.");
            }
            return Content(sb.ToString());
        }

        private async Task<bool> CheckImage(string url)
        {
            return await Task<bool>.Run(async () =>
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(10);
                        HttpResponseMessage response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode && (response.Content.Headers.ContentType.MediaType.StartsWith("image/") || url.EndsWith(".webp")))
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                return false;
            });
        }

        [Route("/broken-links")]
        public async Task<IActionResult> BrokenLinks([FromServices] IPost post, [FromServices] IShortPost shortPost)
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();
            var posts = await post.GetPostsPathTitleViewsAsync();
            foreach (Post item in posts)
            {
                string pageUrl = Url.Action(action: "GetPost", controller: "Home", values: new { path = item.Path }, protocol: "https");
                if (!await CheckPage(pageUrl))
                {
                    count++;
                    sb.Append($"<a href=\"{pageUrl}\" target=\"_blank\">{item.Path}</a> | <a href=\"/update-short-post?id={item.Id}\" target=\"_blank\">Редактировать</a><br />");
                }
            }

            var shortPosts = await shortPost.GetShortPostsPathTitleAsync();
            foreach (ShortPost item in shortPosts)
            {
                string pageUrl = Url.Action(action: "GetPost", controller: "Home", values: new { path = item.Path }, protocol: "https");
                if (!await CheckPage(pageUrl))
                {
                    count++;
                    sb.Append($"<a href=\"{pageUrl}\" target=\"_blank\">{item.Path}</a> | <a href=\"/update-short-post?id={item.Id}\" target=\"_blank\">Редактировать</a><br />");
                }
            }
            Response.ContentType = "text/html;charset=utf-8;";
            if (count == 0)
            {
                return Content("Битых ссылок не найдено.");
            }
            return Content($"Найдены битые ссылки ({count} штук):\n" +sb.ToString());
        }

        private async Task<bool> CheckPage(string url)
        {
            return await Task<bool>.Run(async () =>
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(10);
                        HttpResponseMessage response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            if (response.Content.Headers.ContentType.MediaType.StartsWith("text/html"))
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }

                return false;
            });
        }

        [Route("/image-loading-speed")]
        public async Task<IActionResult> ImageLoadingSpeed([FromServices] IService service)
        {
            StringBuilder sb = new StringBuilder("Скорость загрузки изображений:<br /><br />");
            IEnumerable<BrokenImage> postsImages = await service.GetPostImagesAsync();
            Stopwatch sw = new Stopwatch();
            foreach (BrokenImage item in postsImages)
            {
                sw.Start();
                if (await CheckImage(item.ImageLink))
                {
                    sw.Stop();
                    string time = $"<b>Время: {sw.Elapsed.Seconds} sec, {sw.Elapsed.Milliseconds} ms.</b>";
                    if (sw.Elapsed > TimeSpan.FromMilliseconds(700))
                    {
                        time = $"<b style=\"color:red\">Время: {sw.Elapsed.Seconds} sec, {sw.Elapsed.Milliseconds} ms.</b>";
                    }
                    string pageUrl = Url.Action(action: "GetPost", controller: "Home", values: new { path = item.Path }, protocol: "https");
                    sb.Append($"{time} | <a href=\"{pageUrl}\" target=\"_blank\">{item.Path}</a> | <a href=\"/update?id={item.Id}\" target=\"_blank\">Редактировать</a> | {item.ImageLink}<br />");
                }
                sw.Reset();
            }

            sb.Append("<br /><br /><br />Битые изображения коротких постов:<br /><br />");
            IEnumerable<BrokenImage> shortPostsImages = await service.GetShortPostImagesAsync();
            foreach (BrokenImage item in shortPostsImages)
            {
                sw.Start();
                if (await CheckImage(item.ImageLink))
                {
                    sw.Stop();
                    string time = $"<b>Время: {sw.Elapsed.Seconds} sec, {sw.Elapsed.Milliseconds} ms.</b>";
                    if (sw.Elapsed > TimeSpan.FromMilliseconds(700))
                    {
                        time = $"<b style=\"color:red\">Время: {sw.Elapsed.Seconds} sec, {sw.Elapsed.Milliseconds} ms.</b>";
                    }
                    string pageUrl = Url.Action(action: "GetItem", controller: "ShortPost", values: new { path = item.Path }, protocol: "https");
                    sb.Append($"{time} | <a href=\"{pageUrl}\" target=\"_blank\">{item.Path}</a> | <a href=\"/update-short-post?id={item.Id}\" target=\"_blank\">Редактировать</a> | {item.ImageLink}<br />");
                }
                sw.Reset();
            }
            Response.ContentType = "text/html;charset=utf-8;";
            return Content(sb.ToString());
        }
    }
}
