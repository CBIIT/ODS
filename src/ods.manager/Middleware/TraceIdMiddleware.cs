//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;

namespace Theradex.ODS.Manager.Middleware
{
    //public class TraceIdMiddleware
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly ILogger _logger;

    //    public TraceIdMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    //    {
    //        _next = next;
    //        _logger = loggerFactory.CreateLogger<TraceIdMiddleware>();
    //    }

    //    public async Task Invoke(HttpContext context)
    //    {
    //        using (_logger.BeginScope(new Dictionary<string, object> { ["TraceId"] = "1234"}))
    //        {
    //            await _next(context);
    //        }
    //    }
    //}

    //public static class LoggingMiddlewareExtensions
    //{
    //    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
    //    {
    //        return builder.UseMiddleware<TraceIdMiddleware>();
    //    }
    //}
}
