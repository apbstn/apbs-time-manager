using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Shared.Services
{
    public class Exxception : IExceptionHandler
    {
        private readonly string _message;

        public Exxception(string message)
        {
            _message = message;
        }

        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // Implementation required by IExceptionHandler
            return ValueTask.FromResult(false);
        }
    }

    public class Result
    {
        public bool Success { get; set; }
        public IExceptionHandler ExceptionHandler { get; set; }
    }
}