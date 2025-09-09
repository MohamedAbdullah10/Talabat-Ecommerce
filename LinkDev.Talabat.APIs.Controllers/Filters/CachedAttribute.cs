using LinkDev.Talabat.Core.Domain.Contract.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace LinkDev.Talabat.APIs.Controllers.Filters
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;

        public CachedAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            // Ask CLR for Creating Object from Response Cache Service Explicitly

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var cachedResponse = await responseCacheService.GetCachedResponseAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedResponse)) // No NEED to Execute Endpoint
            {
                var result = new ContentResult()
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = result;
                return;
            }

            // Need to Execute Endpoint

            var executedActionContext = await next.Invoke(); // Execute the Next Action Filter or Action Itself

            if (executedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
                
            {
                await responseCacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            // {{url}}/api/products?pageIndex=1&pageSize=5&sort=name

            StringBuilder keyBuilder = new StringBuilder();

            keyBuilder.Append(request.Path); // /api/products


            // keyBuilder = "/api/products"

            /*Query String Parameters*/
            // pageIndex=1
            // pageSize=5
            // sort=name

            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
                // "/api/products|pageIndex-1"
                // "/api/products|pageIndex-1|pageSize-5"
                // "/api/products|pageIndex-1|pageSize-5|sort-name"
            }

            return keyBuilder.ToString();

        }
    }
}
