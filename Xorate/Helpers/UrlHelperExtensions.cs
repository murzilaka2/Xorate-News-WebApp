using Microsoft.AspNetCore.Mvc;

namespace Xorate.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string AbsoluteRouteUrl(this IUrlHelper urlHelper,
        string routeName, object routeValues = null)
        {
            string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme;
            return urlHelper.RouteUrl(routeName, routeValues, scheme);
        }
    }
}
