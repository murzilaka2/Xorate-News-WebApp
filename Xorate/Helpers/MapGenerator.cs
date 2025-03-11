using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Xml.Linq;
using Xorate.Interfaces;

namespace Xorate.Helpers
{
    public class MapGenerator
    {
        private readonly IPost _posts;
        private readonly IShortPost _shortPosts;

        public MapGenerator(IPost posts, IShortPost shortPosts)
        {
            _posts = posts;
            _shortPosts = shortPosts;
        }

        public async Task<IReadOnlyCollection<string>> GetSitemapNodesAsync(IUrlHelper urlHelper)
        {
            string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme;
            List<string> nodes = new List<string>();
            nodes.Add(urlHelper.AbsoluteRouteUrl("Default", new { controller = "Home", action = "Index" }));
            nodes.Add(urlHelper.AbsoluteRouteUrl("ShortPostsSiteMap"));

            foreach (string path in await _posts.GetPostsPathAsync())
            {
                nodes.Add(urlHelper.AbsoluteRouteUrl("Post", new { path = path }));
            }
            return nodes;
        }

        public async Task<IReadOnlyCollection<string>> GetSitemapShortPostsAsync(IUrlHelper urlHelper)
        {
            string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme;
            List<string> nodes = new List<string>();

            foreach (string path in await _shortPosts.GetShortPostsPathAsync())
            {
                nodes.Add(urlHelper.AbsoluteRouteUrl("ShortPost", new { path = path }));
            }
            return nodes;
        }

        public string GetSitemapDocument(IEnumerable<string> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (string sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode)));
                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);
            return document.ToString();
        }
    }

}
