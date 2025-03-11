using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Xorate.Helpers
{
    public class CustomExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = new RedirectToRouteResult(
                 new RouteValueDictionary
                 {
                    { "controller", "Home" },
                    { "action", "Error" }
                 });
        }
    }
}
