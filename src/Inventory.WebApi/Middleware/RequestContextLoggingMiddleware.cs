using Inventory.Application.Abstractions;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace Inventory.WebApi.Middleware;

public class RequestContextLoggingMiddleware(RequestDelegate next)
{
    public Task Invoke(HttpContext context, ICorrelationIdProvider correlationIdProvider)
    {
        using (LogContext.PushProperty("CorrelationId", correlationIdProvider.GetCorrelationId()))
        {
            return next.Invoke(context);
        }
    }
}
