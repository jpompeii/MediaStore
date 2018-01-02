using MediaContentService.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;

namespace MediaContentService.Security
{
    public class RequestAuth
    {
        private readonly RequestDelegate _next;

        public RequestAuth(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            // context.Request;
            Account acct = MediaStoreContext.GetContext().FindAccount("jpompeii");
            context.Items["Account"] = acct;

            // Call the next delegate/middleware in the pipeline
            Task nextTask = this._next(context);
            MediaStoreContext.FreeContext();
            return nextTask;
        }
    }

    public static class MediaStoreAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseMediaStoreAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestAuth>();
        }
    }


}

