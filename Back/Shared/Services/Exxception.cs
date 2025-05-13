using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Shared.Services
{
    public class Exxception : IExceptionHandler, IExxception
    {
        public string? Message { get; set; }

        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // Implementation required by IExceptionHandler
            return ValueTask.FromResult(false);
        }
    }

    public class Result
    {
        public bool Success { get; set; }
        public Exxception? Exception { get; set; }
    }
}